using HarmonyLib;
using RimWorld;
using System.Linq;
using System.Reflection;
using Verse;
using Verse.AI;

namespace Logistics
{
    [HarmonyPatch]
    public class GotoThing
    {
        static MethodBase TargetMethod() => AccessTools.Method(typeof(Toils_Goto), "GotoThing", new System.Type[]
        {
            typeof(TargetIndex),
            typeof(PathEndMode),
            typeof(bool)
        });

        public static bool Prefix(ref Toil __result, TargetIndex ind, PathEndMode peMode, bool canGotoSpawnedParent)
        {
            Toil toil = ToilMaker.MakeToil("GotoThing");

            bool useInterface = false;
            Pawn actor = null;
            LocalTargetInfo dest = null;
            Thing thing = null;

            toil.initAction = delegate
            {
                actor = toil.actor;
                dest = actor.jobs.curJob.GetTarget(ind);
                thing = dest.Thing;

                if (thing != null)
                {
                    if (thing.def.EverHaulable)
                    {
                        Room room = thing.GetRoom();
                        var interfaces = room.ContainedAndAdjacentThings
                            .Where(i => 
                                i.HasComp<Comp_OutputInterface>() 
                                && i.Spawned
                                && !i.IsForbidden(actor)
                                && actor.CanReserve(thing))
                            .ToList();

                        var closeInterface = interfaces
                            .OrderBy(i => actor.Position.DistanceTo(i.Position))
                            .FirstOrDefault();

                        if (!interfaces.Empty() && actor.Position.DistanceTo(closeInterface.SpawnedParentOrMe.Position) < actor.Position.DistanceTo(thing.Position))
                        {
                            thing = closeInterface;
                            dest = thing;
                            actor.Reserve(thing, actor.jobs.curJob);
                            toil.defaultCompleteMode = ToilCompleteMode.Never;
                            useInterface = true;

                            int postArrivalWait = ((CompProperties_OutputInterface)thing.TryGetComp<Comp_OutputInterface>().props).outputTick;
                            int waitCounter = -1;
                            toil.tickAction = delegate
                            {
                                if (waitCounter == -1 &&!actor.pather.Moving)
                                    waitCounter = 0;
                                if (waitCounter != -1)
                                {
                                    waitCounter++;
                                    if (waitCounter >= postArrivalWait)
                                        actor.jobs.curDriver.ReadyForNextToil();
                                }
                            };
                        }
                    }

                    if (canGotoSpawnedParent)
                        dest = thing.SpawnedParentOrMe;
                }

                actor.pather.StartPath(dest, peMode);
            };

            toil.AddFinishAction(delegate
            {
                if (useInterface)
                {
                    Job job = actor.jobs.curJob;
                    ReservationManager manager = actor.Map.reservationManager;
                    if (manager.ReservedBy(thing, actor))
                        manager.Release(thing, actor, job);
                }
            });

            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            if (canGotoSpawnedParent)
                toil.FailOnSelfAndParentsDespawnedOrNull(ind);
            else
                toil.FailOnDespawnedOrNull(ind);

            __result = toil;
            return false;
        }
    }
}
