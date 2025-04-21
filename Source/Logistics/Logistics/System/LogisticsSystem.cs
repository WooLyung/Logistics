using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Logistics
{
    public static class LogisticsSystem
    {
        public static bool IsAvailableTerminal(ITerminal terminal, Pawn actor = null, bool area = true)
        {
            Thing thing = terminal.Thing;
            if ((actor == null || (!thing.IsForbidden(actor)
                && actor.Map.reachability.CanReach(actor.Position, thing, PathEndMode.Touch, TraverseParms.For(actor, Danger.Some, TraverseMode.ByPawn))))
                && thing.IsActive())
            {
                if (area && actor != null && actor.playerSettings != null && actor.playerSettings.RespectsAllowedArea && !actor.Drafted)
                {
                    Area allowed = actor.playerSettings.EffectiveAreaRestrictionInPawnCurrentMap;
                    if (allowed != null && !allowed[thing.Position])
                        return false;
                }
                return true;
            }
            return false;
        }

        public static Room GetAvailableSystemRoomWithConveyorPort(IConveyorPort port)
        {
            Map map = port.Thing.Map;
            IntVec3 pos = port.Thing.Position;

            Room north = (pos + IntVec3.North).GetRoom(map);
            Room south = (pos + IntVec3.South).GetRoom(map);
            Room east = (pos + IntVec3.East).GetRoom(map);
            Room west = (pos + IntVec3.West).GetRoom(map);

            if (north != null && IsAvailableSystem(north))
                return north;
            if (south != null && IsAvailableSystem(south))
                return south;
            if (east != null && IsAvailableSystem(east))
                return east;
            if (west != null && IsAvailableSystem(west))
                return west;
            return null;
        }
        
        public static bool IsAvailableSystem(Room room)
        {
            if (room.PsychologicallyOutdoors)
                return false;

            return room.GetControllers().Any(controller => controller.Thing.IsActive());
        }

        private static IEnumerable<ITerminal> FindAvailableTerminalsInRoom(Room room, Pawn actor, TerminalType Type)
        {
            if (Type == TerminalType.IO)
                yield break;
            foreach (var terminal in Type == TerminalType.Input
                ? room.GetInputTerminals()
                : room.GetOutputTerminals())
                if (IsAvailableTerminal(terminal, actor))
                    yield return terminal;
        }

        private static IEnumerable<ITerminal> FindAvailableTerminalsWithLinker(Room room, Pawn actor, TerminalType Type)
        {
            if (Type == TerminalType.IO)
                yield break;

            foreach (var linker in room.GetActiveLinkers())
            {
                string target = linker.LinkTargetID;
                var controller = room.Map.GetControllerWithID(target);
                if (controller != null && controller.Thing.IsActive())
                {
                    foreach (var terminal in FindAvailableTerminals(controller.Thing.GetRoom(), actor, Type, false))
                        yield return terminal;
                    yield break;
                }

                foreach (var terminal in Type == TerminalType.Input
                    ? room.Map.GetRemoteInputTerminals()
                    : room.Map.GetRemoteOutputTerminals())
                {
                    if (terminal.NetworkID == target && IsAvailableTerminal(terminal as ITerminal, actor))
                    {
                        yield return terminal as ITerminal;
                        yield break;
                    }
                }
            }
        }

        private static IEnumerable<ITerminal> FindAvailableTerminalsWithConveyorPort(Room room, Pawn actor, TerminalType Type)
        {
            if (Type == TerminalType.IO)
                yield break;

            foreach (IConveyorPort port in room.GetConveyorPorts())
            {
                if (!port.Thing.IsActive())
                    continue;

                foreach (var _terminal in Type == TerminalType.Input
                    ? ConveyorSystem.GetInputs(port)
                    : ConveyorSystem.GetOutputs(port))
                    if (_terminal is ITerminal terminal && IsAvailableTerminal(terminal, actor))
                        yield return terminal;
            }
        }

        public static IEnumerable<ITerminal> FindAvailableTerminals(Room room, Pawn actor, TerminalType Type, bool network = true)
        {
            if (Type == TerminalType.IO || !IsAvailableSystem(room))
                yield break;

            foreach (var terminal in FindAvailableTerminalsInRoom(room, actor, Type))
                yield return terminal;
            foreach (var terminal in FindAvailableTerminalsWithConveyorPort(room, actor, Type))
                yield return terminal;
             if (network)
                foreach (var terminal in FindAvailableTerminalsWithLinker(room, actor, Type))
                    yield return terminal;
        }
        public static ITerminal FindAvailableClosestTerminals(Room room, Pawn actor, TerminalType Type, IntVec3? from = null)
        {
            if (Type == TerminalType.IO)
                return null;

            var terminals = FindAvailableTerminals(room, actor, Type);
            if (terminals.Count() == 0)
                return null;
            return terminals.MinBy(terminal =>
            {
                PawnPath path = FindPath(actor, from ?? actor.Position, terminal.Thing.Position);
                float totalCost = path.TotalCost;
                path.ReleaseToPool();
                return totalCost;
            });
        }

        public static PawnPath FindPath(Pawn pawn, IntVec3 from, IntVec3 to)
        {
            return pawn.Map.pathFinder.FindPath(from, to, TraverseParms.For(pawn, Danger.Some, TraverseMode.ByPawn), PathEndMode.Touch);
        }

        public static bool IsInContainer(this Thing thing)
        {
            if (!thing.def.EverStorable(true))
                return false;

            List<Thing> thingsAtCell = thing.Map.thingGrid.ThingsListAt(thing.Position);
            foreach (Thing t in thingsAtCell)
                if (t is IStorage storage)
                    if (storage.HasThing(t))
                        return true;

            return false;
        }

        public static bool IsInContainer(this LocalTargetInfo target, Map map)
        {
            if (map == null)
                return false;

            List<Thing> thingsAtCell = map.thingGrid.ThingsListAt(target.Cell);
            foreach (Thing t in thingsAtCell)
                if (t is IStorage storage)
                    if (storage.HasThing(t))
                        return true;

            return false;
        }
    }
}
