﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Logistics
{
    public static class LogisticsSystem
    {
        public static IEnumerable<Thing> GetAllItemsInContainer(this Room room)
        {
            foreach (Thing thing in room.ContainedAndAdjacentThings)
                if (thing.IsInContainer())
                    yield return thing;
        }

        public static bool IsAvailableTerminal(Thing t, Pawn actor = null, bool area = true)
        {
            if ((actor == null || (!t.IsForbidden(actor)
                && actor.Map.reachability.CanReach(actor.Position, t.Position, PathEndMode.Touch, TraverseParms.For(actor, Danger.Some, TraverseMode.ByPawn))))
                && t.IsActive())
            {
                if (area && actor != null && actor.playerSettings != null && actor.playerSettings.RespectsAllowedArea && !actor.Drafted)
                {
                    Area allowed = actor.playerSettings.EffectiveAreaRestrictionInPawnCurrentMap;
                    if (allowed != null && !allowed[t.Position])
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

        private static IEnumerable<Thing> FindAvailableTerminalsInRoom<IO>(Room room, Pawn actor) where IO : Comp_Terminal
        {
            foreach (Thing thing in typeof(IO) == typeof(Comp_InputTerminal)
                ? room.GetInputTerminals()
                : room.GetOutputTerminals())
                if (thing.HasComp<IO>() && IsAvailableTerminal(thing, actor))
                    yield return thing;
        }

        private static IEnumerable<Thing> FindAvailableTerminalsWithLinker<IO>(Room room, Pawn actor) where IO : Comp_Terminal
        {
            foreach (var linker in room.GetAllActiveLinkers())
            {
                string target = linker.LinkTargetID;
                var controller = room.Map.GetControllerWithID(target);
                if (controller != null && controller.Thing.IsActive())
                {
                    foreach (var terminal in FindAvailableTerminals<IO>(controller.Thing.GetRoom(), actor, false))
                        yield return terminal;
                    yield break;
                }

                foreach (var terminal in typeof(IO) == typeof(Comp_InputTerminal) 
                    ? room.Map.GetRemoteInputTerminals()
                    : room.Map.GetRemoteOutputTerminals())
                {
                    if (terminal.NetworkID == target && IsAvailableTerminal(terminal, actor))
                    {
                        yield return terminal;
                        yield break;
                    }
                }
            }
        }

        private static IEnumerable<Thing> FindAvailableTerminalsWithConveyorPort<IO>(Room room, Pawn actor) where IO : Comp_Terminal
        {
            foreach (IConveyorPort port in room.GetConveyorPorts())
            {
                if (!port.Thing.IsActive())
                    continue;

                foreach (var _terminal in typeof(IO) == typeof(Comp_InputTerminal)
                    ? ConveyorSystem.GetInputs(port)
                    : ConveyorSystem.GetOutputs(port))
                {
                    if (!(_terminal is Thing))
                        continue;

                    Thing terminal = (Thing)_terminal;
                    if (terminal.HasComp<IO>() && IsAvailableTerminal(terminal, actor))
                        yield return terminal;
                }    
            }
        }

        public static IEnumerable<Thing> FindAvailableTerminals<IO>(Room room, Pawn actor, bool network = true) where IO : Comp_Terminal
        {
            if (!IsAvailableSystem(room))
                yield break;

            foreach (Thing terminal in FindAvailableTerminalsInRoom<IO>(room, actor))
                yield return terminal;
            foreach (Thing terminal in FindAvailableTerminalsWithConveyorPort<IO>(room, actor))
                yield return terminal;
             if (network)
                foreach (Thing terminal in FindAvailableTerminalsWithLinker<IO>(room, actor))
                    yield return terminal;
        }
        public static Thing FindAvailableClosestTerminals<IO>(Room room, Pawn actor, IntVec3? from = null) where IO : Comp_Terminal
        {
            var terminals = FindAvailableTerminals<IO>(room, actor);
            if (terminals.Count() == 0)
                return null;
            return terminals.MinBy(t =>
            {
                PawnPath path = FindPath(actor, from ?? actor.Position, t.Position);
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
                if (t.HasComp<Comp_LogisticsContainer>())
                    return true;
            return false;
        }

        public static bool IsInContainer(this LocalTargetInfo target, Map map)
        {
            if (map == null)
                return false;

            List<Thing> thingsAtCell = map.thingGrid.ThingsListAt(target.Cell);
            foreach (Thing t in thingsAtCell)
                if (t.HasComp<Comp_LogisticsContainer>())
                    return true;

            return false;
        }
    }
}
