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
            return room.ContainedAndAdjacentThings.Any(t => t is Building_LogisticsSystemController controller && controller.IsOperational());
        }

        private static IEnumerable<Thing> FindAvailableInterfacesInSystem<IO>(Room room, Pawn actor) where IO : Comp_Interface
        {
            foreach (Thing t in room.ContainedAndAdjacentThings)
            {
                if (t.HasComp<IO>() && IsAvailableInterface(t, actor))
                    yield return t;
            }
        }

        private static IEnumerable<Thing> FindAvailableInterfacesWithLinker<IO>(Room room, Pawn actor) where IO : Comp_Interface
        {
            foreach (Thing t in room.ContainedAndAdjacentThings)
            {
                if (t is Building_LogisticsNetworkLinker linker && linker.IsOperational())
                {
                    string target = linker.Target;
                    var ls = linker.Map.listerThings.ThingsOfDef(ThingDef.Named("LogisticsSystemController"));

                    foreach (var t2 in ls)
                    {
                        if (t2 is NetworkDevice device)
                        {
                            if (device.NetworkID == target)
                            {
                                var ls2 = FindAvailableInterfaces<IO>(device.GetRoom(), actor, false);
                                foreach (var itf in ls2)
                                    yield return itf;
                                yield break;
                            }
                        }
                    }

                    var ls3 = linker.Map.listerThings.ThingsOfDef(ThingDef.Named(typeof(IO) == typeof(Comp_InputInterface) ? "RemoteInputInterface" : "RemoteOutputInterface"));
                    var ls4 = linker.Map.listerThings.ThingsOfDef(ThingDef.Named("RemoteIOInterface"));
                    ls3.AddRange(ls4);

                    foreach (var t3 in ls3)
                    {
                        if (t3 is NetworkDevice device)
                        {
                            if (device.NetworkID == target && IsAvailableInterface(device, actor))
                            {
                                yield return t3;
                                yield break;
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<Thing> FindAvailableInterfacesWithConveyor<IO>(Room room, Pawn actor) where IO : Comp_Interface
        {
            foreach (Thing t in room.ContainedAndAdjacentThings)
            {
                if (t is Building_ConveyorInterface && IsAvailableInterface(t, actor, area: false))
                {
                    Stack<IntVec3> stack = new Stack<IntVec3>();
                    HashSet<IntVec3> visited = new HashSet<IntVec3>();

                    foreach (IntVec3 d in allDirs)
                    {
                        IntVec3 v = t.Position + d;

                        Thing itf = v.GetThingList(t.Map).FirstOrDefault(t2 => t2.HasComp<IO>());
                        if (itf != null && IsAvailableInterface(itf))
                            yield return itf;

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
                                    yield return itf;

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
                                        yield return itf;

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

        public static IEnumerable<Thing> FindAvailableInterfaces<IO>(Room room, Pawn actor, bool network = true) where IO : Comp_Interface
        {
            if (!IsAvailableSystem(room))
                yield break;

            foreach (Thing itf in FindAvailableInterfacesInSystem<IO>(room, actor))
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
