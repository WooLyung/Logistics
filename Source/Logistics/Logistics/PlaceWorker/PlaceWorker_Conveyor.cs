using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Logistics
{
    public class PlaceWorker_Conveyor : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 center, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            List<Thing> list = map.thingGrid.ThingsListAt(center);
            for (int i = 0; i < list.Count; i++)
            {
                Thing thing2 = list[i];
                if (thing2 != thingToIgnore && ((thing2.def.category == ThingCategory.Building && thing2.def == def) || ((thing2.def.IsBlueprint || thing2.def.IsFrame) && thing2.def.entityDefToBuild is ThingDef thingDef && thingDef == def)))
                {
                    return "IdenticalThingExists".Translate();
                }
            }

            return true;
        }
    }
}
