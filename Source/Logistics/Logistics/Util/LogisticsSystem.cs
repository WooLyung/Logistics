﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Logistics.Util
{
    public static class LogisticsSystem
    {
        public static IEnumerable<Thing> FindAvailableOutputInterfaces(Room room, Pawn actor = null)
        {
            if (!room.ContainedAndAdjacentThings.Any(t => t is Building_LogisticsSystemController controller && controller.IsOperational()))
                yield break;

            foreach (Thing t in room.ContainedAndAdjacentThings)
            {
                if (t.HasComp<Comp_OutputInterface>()
                    && (actor == null || (!t.IsForbidden(actor) && actor.CanReserve(t)
                    && actor.Map.reachability.CanReach(actor.Position, t.Position, PathEndMode.Touch, TraverseParms.For(actor, Danger.Some, TraverseMode.ByPawn))))
                    && t.IsOperational())
                {
                    if (actor != null && actor.playerSettings != null && actor.playerSettings.RespectsAllowedArea && !actor.Drafted)
                    {
                        Area allowed = actor.playerSettings.EffectiveAreaRestrictionInPawnCurrentMap;
                        if (allowed != null && !allowed[t.Position])
                            continue;
                    }
                    yield return t;
                }
            }
        }

        public static IEnumerable<Thing> FindAvailableInputInterfaces(Room room, Pawn actor = null)
        {
            if (!room.ContainedAndAdjacentThings.Any(t => t is Building_LogisticsSystemController controller && controller.IsOperational()))
                yield break;

            foreach (Thing t in room.ContainedAndAdjacentThings)
            {
                if (t.HasComp<Comp_InputInterface>()
                    && (actor == null || (!t.IsForbidden(actor) && actor.CanReserve(t)
                    && actor.Map.reachability.CanReach(actor.Position, t.Position, PathEndMode.Touch, TraverseParms.For(actor, Danger.Some, TraverseMode.ByPawn))))
                    && t.IsOperational())
                {
                    if (actor != null && actor.playerSettings != null && actor.playerSettings.RespectsAllowedArea && !actor.Drafted)
                    {
                        Area allowed = actor.playerSettings.EffectiveAreaRestrictionInPawnCurrentMap;
                        if (allowed != null && !allowed[t.Position])
                            continue;
                    }
                    yield return t;
                }
            }
        }

        public static Thing FindAvailableClosestOutputInterface(Room room, Pawn actor)
        {
            var interfaces = FindAvailableOutputInterfaces(room, actor);
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

        public static Thing FindAvailableClosestInputInterface(Room room, Pawn actor)
        {
            var interfaces = FindAvailableInputInterfaces(room, actor);
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
