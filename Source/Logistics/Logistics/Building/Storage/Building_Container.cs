using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Logistics
{
    public class Building_Container : Building_Storage, IStorage
    {
        public Thing Thing => this;

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

        public Thing GetAnyStack(StorageSettings filter)
        {
            foreach (Thing thing in StoredThings)
                if (filter.AllowedToAccept(thing))
                    return thing;
            return null;
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
                int cnt = cell.GetThingList(map).Count(item => item.def.EverStorable(true));
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
