using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Logistics.Util
{
    public static class LogisticsSystem
    {
        public static IEnumerable<Thing> FindAvailableOutputInterfacesSingle(Room room, Pawn actor = null)
        {
            if (!room.ContainedAndAdjacentThings.Any(t => t is Building_LogisticsSystemController controller && controller.IsOperational()))
                yield break;

            foreach (Thing t in room.ContainedAndAdjacentThings)
            {
                if (t.HasComp<Comp_OutputInterface>()
                    && (actor == null || (!t.IsForbidden(actor) && actor.CanReserve(t)
                    && actor.Map.reachability.CanReach(actor.Position, t.Position, PathEndMode.Touch, TraverseParms.For(actor, Danger.Some, TraverseMode.ByPawn))))
                    && t.IsOperational())
                    yield return t;
            }
        }

        public static IEnumerable<Thing> FindAvailableInputInterfacesSingle(Room room, Pawn actor = null)
        {
            if (!room.ContainedAndAdjacentThings.Any(t => t is Building_LogisticsSystemController controller && controller.IsOperational()))
                yield break;

            foreach (Thing t in room.ContainedAndAdjacentThings)
            {
                if (t.HasComp<Comp_InputInterface>()
                    && (actor == null || (!t.IsForbidden(actor) && actor.CanReserve(t)
                    && actor.Map.reachability.CanReach(actor.Position, t.Position, PathEndMode.Touch, TraverseParms.For(actor, Danger.Some, TraverseMode.ByPawn))))
                    && t.IsOperational())
                    yield return t;
            }
        }

        public static IEnumerable<Thing> FindAvailableOutputInterfacesSingle(Thing thing, Pawn actor = null)
        {
            return FindAvailableOutputInterfacesSingle(thing.GetRoom(), actor);
        }

        public static IEnumerable<Thing> FindAvailableInputInterfacesSingle(Thing thing, Pawn actor = null)
        {
            return FindAvailableInputInterfacesSingle(thing.GetRoom(), actor);
        }

        public static Thing FindAvailableClosestOutputInterfaceSingle(Room room, Pawn actor)
        {
            var interfaces = FindAvailableOutputInterfacesSingle(room, actor);
            if (interfaces.Count() == 0)
                return null;
            return interfaces.MinBy(t =>
            {
                PawnPath path = FindPath(actor, t.Position);
                float totalCost = path.TotalCost;
                path.ReleaseToPool();
                return totalCost;
            });
        }

        public static Thing FindAvailableClosestInputInterfaceSingle(Room room, Pawn actor)
        {
            var interfaces = FindAvailableInputInterfacesSingle(room, actor);
            if (interfaces.Count() == 0)
                return null;
            return interfaces.MinBy(t =>
            {
                PawnPath path = FindPath(actor, t.Position);
                float totalCost = path.TotalCost;
                path.ReleaseToPool();
                return totalCost;
            });
        }

        public static PawnPath FindPath(Pawn pawn, IntVec3 to)
        {
            return pawn.Map.pathFinder.FindPath(pawn.Position, to, TraverseParms.For(pawn, Danger.Some, TraverseMode.ByPawn), PathEndMode.Touch);
        }

        public static bool IsInContainer(this Thing thing)
        {
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
