using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Logistics
{
    public class Building_Container : Building_Storage, IStorage
    {
        public Thing Thing => this;
        public bool IsActive => true;
        public IEnumerable<Thing> StoredThings => slotGroup.HeldThings;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            LCache.GetLCache(map).AddStorage(this);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            LCache.GetLCache(Map).RemoveStorage(this);
            base.DeSpawn(mode);
        }

        public Thing GetAnyStack(ThingFilter filter1, StorageSettings filter2)
        {
            foreach (Thing thing in StoredThings)
                if (thing.stackCount > 0 && (filter1 == null || filter1.Allows(thing)) && (filter2 == null || filter2.AllowedToAccept(thing)))
                    return thing;
            return null;
        }

        public int TryConsume(ThingFilter filter1, StorageSettings filter2, int max)
        {
            int cnt = 0;
            while (max > 0)
            {
                Thing thing = GetAnyStack(filter1, filter2);
                if (thing == null)
                    return cnt;

                if (thing.stackCount >= max)
                {
                    thing.SplitOff(max).Destroy();
                    cnt += max;
                    return cnt;
                }
                else
                {
                    max -= thing.stackCount;
                    cnt += thing.stackCount;
                    thing.Destroy();
                }
            }
            return cnt;
        }

        public bool HasThing(Thing thing)
        {
            foreach (Thing t in StoredThings)
                if (thing == t)
                    return true;
            return false;
        }

        public bool TryInsert(Thing thing, out int remained)
        {
            if (!GetStoreSettings().AllowedToAccept(thing))
            {
                remained = thing.stackCount;
                return false;
            }

            int stackCount = thing.stackCount;
            foreach (var held in StoredThings)
            {
                if (!held.CanStackWith(thing))
                    continue;

                int space = held.def.stackLimit - held.stackCount;
                if (thing.stackCount <= space)
                {
                    held.TryAbsorbStack(thing, true);
                    remained = 0;
                    return true;
                }
                else if (space >= 1)
                    held.TryAbsorbStack(thing.SplitOff(space), true);
            }
            
            Map map = Map;
            ThingGrid thingGrid = map.thingGrid;

            foreach (var cell in GetSlotGroup().CellsList)
            {
                int cnt = cell.GetThingList(map).Count(item => item.def.EverHaulable);
                if (def.building.maxItemsInCell > cnt)
                {
                    if (thing.Spawned)
                        thing.DeSpawn();
                    GenPlace.TryPlaceThing(thing, cell, map, ThingPlaceMode.Direct);
                    remained = 0;
                    return true;
                }
            }

            remained = thing.stackCount;
            return remained != stackCount;
        }
    }
}
