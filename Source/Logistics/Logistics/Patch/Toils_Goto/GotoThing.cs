using HarmonyLib;
using RimWorld;
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
                    if (thing.IsInContainer())
                    {
                        Room room = thing.GetRoom();
                        var closest = LogisticsSystem.FindAvailableClosestTerminals(thing.GetRoom(), actor, TerminalType.Output);

                        if (closest != null)
                        {
                            PawnPath path1 = LogisticsSystem.FindPath(actor, actor.Position, closest.Thing.Position);
                            PawnPath path2 = LogisticsSystem.FindPath(actor, actor.Position, thing.Position);
                            float cost1 = path1.TotalCost;
                            float cost2 = path2.TotalCost;
                            path1.ReleaseToPool();
                            path2.ReleaseToPool();

                            if (cost1 < cost2)
                            {
                                thing = closest.Thing;
                                dest = thing;
                            }
                        }
                    }

                    if (canGotoSpawnedParent)
                        dest = thing.SpawnedParentOrMe;
                }

                actor.pather.StartPath(dest, peMode);
            };

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
