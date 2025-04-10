using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using Verse.AI;

namespace Logistics
{
    [HarmonyPatch]
    public class MakeNewToils
    {
        static MethodBase TargetMethod() => AccessTools.Method(typeof(JobDriver_HaulToCell), "MakeNewToils");

        public static bool Prefix(ref IEnumerable<Toil> __result, JobDriver_HaulToCell __instance)
        {
            Job job = __instance.job;
            LocalTargetInfo cell = job.GetTarget(TargetIndex.B);
            Map map = __instance.pawn.Map;

            if (cell.IsInContainer(map))
            {
                __result = MakeNewToilsWithContainer(__instance);
                return false;
            }
            return true;
        }

        private static IEnumerable<Toil> MakeNewToilsWithContainer(JobDriver_HaulToCell __instance)
        {
            __instance.FailOnDestroyedOrNull(TargetIndex.A);
            __instance.FailOnBurningImmobile(TargetIndex.B);
            bool forbiddenInitially = (bool)AccessTools.Field(typeof(JobDriver_HaulToCell), "forbiddenInitially").GetValue(__instance);
            if (!forbiddenInitially)
                __instance.FailOnForbidden(TargetIndex.A);

            yield return Toils_General.DoAtomic(delegate
            {
                AccessTools.Field(typeof(JobDriver_HaulToCell), "startTick").SetValue(__instance, Find.TickManager.TicksGame);
            });
            Toil reserveTargetA = Toils_Reserve.Reserve(TargetIndex.A);
            yield return reserveTargetA;
            Toil postCarry = Toils_General.Label();
            Thing carriedThing;
            yield return Toils_Jump.JumpIf(postCarry, () => (carriedThing = __instance.pawn.carryTracker.CarriedThing) != null && carriedThing == __instance.pawn.jobs.curJob.GetTarget(TargetIndex.A).Thing);
            yield return Toils_General.DoAtomic(delegate
            {
                bool dropCarriedThingIfNotTarget = (bool)AccessTools.Method(typeof(JobDriver_HaulToCell), "get_DropCarriedThingIfNotTarget").Invoke(__instance, null);
                if (dropCarriedThingIfNotTarget && __instance.pawn.IsCarrying())
                {
                    if (DebugViewSettings.logCarriedBetweenJobs)
                        Log.Message($"Dropping {__instance.pawn.carryTracker.CarriedThing} because it is not the designated Thing to haul.");
                    __instance.pawn.carryTracker.TryDropCarriedThing(__instance.pawn.Position, ThingPlaceMode.Near, out var _);
                }
            });

            Toil toilGoto = null;
            toilGoto = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch, canGotoSpawnedParent: true).FailOnSomeonePhysicallyInteracting(TargetIndex.A).FailOn((Func<bool>)delegate
            {
                Pawn actor = toilGoto.actor;
                Job curJob = actor.jobs.curJob;
                if (curJob.haulMode == HaulMode.ToCellStorage)
                {
                    Thing thing = curJob.GetTarget(TargetIndex.A).Thing;
                    if (!actor.jobs.curJob.GetTarget(TargetIndex.B).Cell.IsValidStorageFor(__instance.pawn.Map, thing))
                        return true;
                }
                return false;
            });
            yield return toilGoto;
            yield return Toils_Haul.StartCarryThing(TargetIndex.A, putRemainderInQueue: false, subtractNumTakenFromJobCount: true, failIfStackCountLessThanJobCount: false, reserve: true, HaulAIUtility.IsInHaulableInventory(__instance.ToHaul));
            yield return postCarry;
            if (__instance.job.haulOpportunisticDuplicates)
            {
                yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveTargetA, TargetIndex.A, TargetIndex.B);
            }

            Job job = __instance.job;
            Pawn actor2 = __instance.pawn;
            Map map = actor2.Map;
            LocalTargetInfo cell = job.GetTarget(TargetIndex.B);
            Room room = RegionAndRoomQuery.RoomAt(cell.Cell, map);
            var closest = LogisticsSystem.FindAvailableClosestInterface<Comp_InputInterface>(room, actor2);

            float cost1 = 0, cost2 = 0;
            if (closest != null)
            {
                PawnPath path1 = LogisticsSystem.FindPath(actor2, closest.Position);
                PawnPath path2 = LogisticsSystem.FindPath(actor2, cell.Cell);
                cost1 = path1.TotalCost;
                cost2 = path2.TotalCost;
                path1.ReleaseToPool();
                path2.ReleaseToPool();
            }

            Toil carryToCell;
            if (closest != null && cost1 < cost2)
            {
                int postArrivalWait = ((CompProperties_InputInterface)closest.TryGetComp<Comp_InputInterface>().props).inputTick;
                job.SetTarget(TargetIndex.C, closest);
                carryToCell = Toils.CarryHauledThingToInterface(closest, TargetIndex.B, PathEndMode.ClosestTouch);

                yield return Toils_Reserve.Reserve(TargetIndex.C);
                yield return carryToCell;
                yield return Toils_General.WaitWith(TargetIndex.C, postArrivalWait, true);
            }
            else
            {
                carryToCell = Toils_Haul.CarryHauledThingToCell(TargetIndex.B);
                yield return carryToCell;
            }
            yield return (Toil)AccessTools.Method(typeof(JobDriver_HaulToCell), "PossiblyDelay").Invoke(__instance, null);
            yield return (Toil)AccessTools.Method(typeof(JobDriver_HaulToCell), "BeforeDrop").Invoke(__instance, null);
            yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.B, carryToCell, storageMode: true);
        }
    }
}
