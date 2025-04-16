using RimWorld;
using System.Collections.Generic;
using System;
using Verse;
using System.Linq;
using Verse.Noise;

namespace Logistics
{
    public static class Translator
    {
        public static bool ToWarehouseAny(Thing thing, Room room)
        {
            var containers = new List<Building_Storage>();
            bool flag = false;

            foreach (var t in room.ContainedAndAdjacentThings)
            {
                var comp = t.TryGetComp<Comp_LogisticsContainer>();
                if (comp != null && comp.parent != null && comp.parent.Spawned && comp.parent is Building_Storage container)
                    containers.Add(container);
            }

            foreach (var container in containers)
            {
                foreach (var held in container.GetSlotGroup().HeldThings)
                {
                    if (held.CanStackWith(thing))
                    {
                        int space = held.def.stackLimit - held.stackCount;
                        if (thing.stackCount <= space)
                        {
                            held.TryAbsorbStack(thing, true);
                            return true;
                        }
                        else if (space >= 1)
                        {
                            held.TryAbsorbStack(thing.SplitOff(space), true);
                            flag = true;
                        }
                    }
                }
            }

            ThingGrid thingGrid = thing.Map.thingGrid;
            Map map = thing.Map;
            foreach (var container in containers)
            {
                SlotGroup slotGroup = container.GetSlotGroup();
                foreach (var cell in slotGroup.CellsList)
                {
                    int cnt = cell.GetThingList(map).Count(item => item.def.EverStorable(true));
                    if (container.def.building.maxItemsInCell > cnt)
                    {
                        thing.DeSpawn();
                        GenPlace.TryPlaceThing(thing, cell, map, ThingPlaceMode.Direct);
                        return true;
                    }
                }
            }

            return flag;
        }
    }
}
