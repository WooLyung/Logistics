using Verse.AI;
using Verse;
using RimWorld;
using UnityEngine;

namespace Logistics
{
    public static class Toils
    {
        public static Toil CarryHauledThingToInterface(Thing itf, TargetIndex squareIndex, PathEndMode pathEndMode = PathEndMode.ClosestTouch)
        {
            Toil toil = ToilMaker.MakeToil("CarryHauledThingToInterface");
            toil.initAction = delegate
            {
                toil.actor.pather.StartPath(itf, pathEndMode);
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            toil.AddEndCondition(delegate
            {
                Pawn actor2 = toil.actor;
                IntVec3 cell2 = actor2.jobs.curJob.GetTarget(squareIndex).Cell;
                CompPushable compPushable2 = actor2.carryTracker.CarriedThing.TryGetComp<CompPushable>();
                if (compPushable2 != null)
                {
                    Vector3 v = actor2.Position.ToVector3() + compPushable2.drawPos;
                    if (new IntVec3(v) == cell2)
                    {
                        return JobCondition.Succeeded;
                    }
                }
                return JobCondition.Ongoing;
            });
            toil.AddFailCondition(delegate
            {
                Pawn actor = toil.actor;
                IntVec3 cell = actor.jobs.curJob.GetTarget(squareIndex).Cell;
                if (actor.jobs.curJob.haulMode == HaulMode.ToCellStorage && !cell.IsValidStorageFor(actor.Map, actor.carryTracker.CarriedThing))
                {
                    return true;
                }

                CompPushable compPushable = actor.carryTracker.CarriedThing.TryGetComp<CompPushable>();
                return (compPushable != null && !compPushable.canBePushed) ? true : false;
            });
            return toil;
        }
    }
}
