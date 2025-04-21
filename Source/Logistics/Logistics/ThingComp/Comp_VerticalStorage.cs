using RimWorld;
using System.Collections.Generic;
using Verse;

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

        public IEnumerable<Thing> StoredThings => innerContainer;

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

        public bool TryInsert(Thing thing, out int remained)
        {
            if (innerContainer.Count != 0 && GetItemDef() != thing.def)
            {
                remained = thing.stackCount;
                return false;
            }

            int space = 0;
            foreach (Thing t in innerContainer)
                if (t.CanStackWith(thing))
                    space += thing.def.stackLimit - t.stackCount;

            if (space >= thing.stackCount || innerContainer.Count < ((CompProperties_VerticalStorage)props).maxStack)
            {
                if (thing.Spawned)
                    thing.DeSpawn();
                innerContainer.TryAdd(thing, true);
                remained = 0;
                return true;
            }
            else if (space > 0)
            {
                remained = thing.stackCount - space;
                innerContainer.TryAdd(thing.SplitOff(space), true);
                return true;
            }

            remained = thing.stackCount;
            return false;
        }
    }
}
