using RimWorld;
using System.Collections.Generic;
using Verse;
using static HarmonyLib.Code;

namespace Logistics
{
    public class Comp_VerticalStorage : ThingComp, IThingHolder
    {
        public ThingOwner innerContainer;

        public Comp_VerticalStorage()
        {
            innerContainer = new ThingOwner<Thing>(this);
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return innerContainer;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
        }

        public override void CompTick()
        {
            base.CompTick();
            innerContainer.ThingOwnerTick();
        }

        public bool HasItem()
        {
            return innerContainer.Count > 0;
        }

        public ThingDef GetItemDef()
        {
            if (!HasItem())
                return null;
            return innerContainer[0].def;
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            innerContainer.TryDropAll(parent.Position, map, ThingPlaceMode.Near);
        }

        public int AddItemAny(Thing thing)
        {
            if (innerContainer.Count != 0 && GetItemDef() != thing.def)
                return thing.stackCount;

            int remained = 0;
            foreach (Thing t in innerContainer)
                if (t.CanStackWith(thing))
                    remained += thing.def.stackLimit - t.stackCount;

            if (remained >= thing.stackCount)
            {
                if (thing.Spawned)
                    thing.DeSpawn();
                innerContainer.TryAdd(thing, true);
                return 0;
            }
            else
            {
                if (remained > 0)
                {
                    Thing split = thing.SplitOff(remained);
                    innerContainer.TryAdd(split, true);
                }

                if (innerContainer.Count < ((CompProperties_VerticalStorage)props).maxStack)
                {
                    if (thing.Spawned)
                        thing.DeSpawn();
                    innerContainer.TryAdd(thing, true);
                    return 0;
                }

                return thing.stackCount;
            }
        }
    }
}
