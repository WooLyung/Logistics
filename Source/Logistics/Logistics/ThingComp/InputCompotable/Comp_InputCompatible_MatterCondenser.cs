using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace Logistics
{
    public class Comp_InputCompatible_MatterCondenser : ThingComp, IComp_InputCompotable
    {
        public bool TryExtract(Room room, bool network = true)
        {
            if (!(parent is Building_MatterCondenser))
                return false;

            Building_MatterCondenser condenser = parent as Building_MatterCondenser;
            if (!condenser.CanExtract())
                return false;

            ThingDef def = DefDatabase<ThingDef>.GetNamed("MatterCore");
            Thing thing = ThingMaker.MakeThing(def);

            foreach (IStorage storage in room.GetActiveStorages(network))
            {
                int remained;
                if (storage.TryInsert(thing, out remained))
                {
                    condenser.Flush();
                    return true;
                }
            }

            thing.Destroy();
            return false;
        }
    }
}
