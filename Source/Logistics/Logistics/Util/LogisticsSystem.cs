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
            if ((actor == null || (!t.IsForbidden(actor) && actor.CanReserve(t)
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
            return room.ContainedAndAdjacentThings.Any(t => t is Building_LogisticsSystemController controller && controller.IsOperational());
        }

        public static IEnumerable<Thing> FindAvailableInterfaces<IO>(Room room, Pawn actor = null) where IO : Comp_Interface
        {
            if (!IsAvailableSystem(room))
                yield break;

            foreach (Thing t in room.ContainedAndAdjacentThings)
            {
                if (t.HasComp<IO>() && IsAvailableInterface(t, actor))
                    yield return t;
                if (t is Building_ConveyorInterface && IsAvailableInterface(t, actor, area: false))
                {
                    Stack<IntVec3> stack = new Stack<IntVec3>();
                    HashSet<IntVec3> visited = new HashSet<IntVec3>();

                    foreach (IntVec3 d in allDirs)
                    {
                        IntVec3 v = t.Position + d;

                        Thing itf = v.GetThingList(t.Map).FirstOrDefault(t2 => t2.HasComp<IO>());
                        if (itf != null && IsAvailableInterface(itf))
                        {
                            yield return itf;
                            continue;
                        }

                        Thing conveyor = v.GetThingList(t.Map).FirstOrDefault(t2 => t2.HasComp<Comp_Conveyor>());
                        if (conveyor != null)
                        {
                            if (typeof(IO) == typeof(Comp_OutputInterface) && d == conveyor.Rotation.FacingCell
                                || typeof(IO) == typeof(Comp_InputInterface) && -d == conveyor.Rotation.FacingCell)
                            {
                                visited.Add(v);
                                stack.Push(v);
                            }
                        }
                    }

                    while (stack.Count > 0)
                    {
                        IntVec3 current = stack.Pop();
                        Thing conveyor = current.GetThingList(t.Map).FirstOrDefault(t2 => t2.HasComp<Comp_Conveyor>());
    
                        if (typeof(IO) == typeof(Comp_OutputInterface))
                        {
                            IntVec3 d = conveyor.Rotation.FacingCell;
                            IntVec3 v = current + d;
                            if (!visited.Contains(v))
                            {
                                Thing itf = v.GetThingList(t.Map).FirstOrDefault(t2 => t2.HasComp<IO>());
                                if (itf != null && IsAvailableInterface(itf))
                                {
                                    yield return itf;
                                    continue;
                                }

                                Thing conveyor2 = v.GetThingList(t.Map).FirstOrDefault(t2 => t2.HasComp<Comp_Conveyor>());
                                if (conveyor2 != null)
                                {
                                    visited.Add(v);
                                    stack.Push(v);
                                }
                            }
                        }
                        else if (typeof(IO) == typeof(Comp_InputInterface))
                        {
                            foreach (IntVec3 d in allDirs)
                            {
                                IntVec3 v = current + d;
                                if (!visited.Contains(v))
                                {
                                    Thing itf = v.GetThingList(t.Map).FirstOrDefault(t2 => t2.HasComp<IO>());
                                    if (itf != null && IsAvailableInterface(itf))
                                    {
                                        yield return itf;
                                        continue;
                                    }

                                    Thing conveyor2 = v.GetThingList(t.Map).FirstOrDefault(t2 => t2.HasComp<Comp_Conveyor>());
                                    if (conveyor2 != null && -d == conveyor2.Rotation.FacingCell)
                                    {
                                        visited.Add(v);
                                        stack.Push(v);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static Thing FindAvailableClosestInterface<IO>(Room room, Pawn actor) where IO : Comp_Interface
        {
            var interfaces = FindAvailableInterfaces<IO>(room, actor);
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
