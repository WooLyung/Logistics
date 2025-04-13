using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Logistics
{
    public static class LogisticsSystem
    {
        private static IntVec3[] allDirs = { IntVec3.North, IntVec3.South, IntVec3.East, IntVec3.West };

        public static bool IsAvailableInterface(Thing t, Pawn actor = null, bool area = true)
        {
            if ((actor == null || (!t.IsForbidden(actor)
                && actor.Map.reachability.CanReach(actor.Position, t.Position, PathEndMode.Touch, TraverseParms.For(actor, Danger.Some, TraverseMode.ByPawn))))
                && t.IsOperational())
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

        public static bool IsAvailableSystem(Room room)
        {
            if (room.PsychologicallyOutdoors)
                return false;

            return room.ThingsOfDef(LogisticsThingDefOf.LogisticsSystemController)
                .Any(controller => controller.GetRoom() == room && controller.IsOperational());
        }

        private static IEnumerable<Thing> FindAvailableInterfacesInRoom<IO>(Room room, Pawn actor) where IO : Comp_Interface
        {
            foreach (Thing thing in typeof(IO) == typeof(Comp_InputInterface)
                ? room.GetAllInputInterfaces()
                : room.GetAllOutputInterfaces())
                if (thing.HasComp<IO>() && IsAvailableInterface(thing, actor))
                    yield return thing;
        }

        private static IEnumerable<Thing> FindAvailableInterfacesWithLinker<IO>(Room room, Pawn actor) where IO : Comp_Interface
        {
            foreach (var linker in room.GetAllOperationalLinkers())
            {
                string target = linker.Target;
                var controller = room.Map.GetControllerWithID(target);
                if (controller != null && controller.IsOperational())
                {
                    foreach (var itf in FindAvailableInterfaces<IO>(controller.GetRoom(), actor, false))
                        yield return itf;
                    yield break;
                }

                foreach (var itf in typeof(IO) == typeof(Comp_InputInterface) 
                    ? room.Map.GetAllRemoteInputInterface()
                    : room.Map.GetAllRemoteOutputInterface())
                {
                    if (itf.NetworkID == target && IsAvailableInterface(itf, actor))
                    {
                        yield return itf;
                        yield break;
                    }
                }
            }
        }

        private static IEnumerable<Thing> FindAvailableInterfacesWithConveyor<IO>(Room room, Pawn actor) where IO : Comp_Interface
        {
            foreach (Building_ConveyorInterface convItf in room.GetAllConveyorInterfaces())
            {
                foreach (var itf in typeof(IO) == typeof(Comp_InputInterface)
                    ? ConveyorSystem.GetInputs(convItf)
                    : ConveyorSystem.GetOutputs(convItf))
                    if (itf.HasComp<IO>() && IsAvailableInterface(itf, actor))
                        yield return itf;
            }
        }

        public static IEnumerable<Thing> FindAvailableInterfaces<IO>(Room room, Pawn actor, bool network = true) where IO : Comp_Interface
        {
            if (!IsAvailableSystem(room))
                yield break;

            foreach (Thing itf in FindAvailableInterfacesInRoom<IO>(room, actor))
                yield return itf;
            foreach (Thing itf in FindAvailableInterfacesWithConveyor<IO>(room, actor))
                yield return itf;
             if (network)
                foreach (Thing itf in FindAvailableInterfacesWithLinker<IO>(room, actor))
                    yield return itf;
        }
        public static Thing FindAvailableClosestInterface<IO>(Room room, Pawn actor, IntVec3? from = null) where IO : Comp_Interface
        {
            var interfaces = FindAvailableInterfaces<IO>(room, actor);
            if (interfaces.Count() == 0)
                return null;
            return interfaces.MinBy(t =>
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
            if (!thing.def.EverHaulable)
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
