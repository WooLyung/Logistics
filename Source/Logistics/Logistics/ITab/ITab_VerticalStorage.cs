using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Logistics
{
    public class ITab_VerticalStorage : ITab_ContentsBase
    {
        public override IList<Thing> container => Storage.innerContainer;
        public Comp_VerticalStorage Storage => SelThing.TryGetComp<Comp_VerticalStorage>();

        public override bool IsVisible
        {
            get
            {
                if (SelThing != null && (SelThing.Faction == null || SelThing.Faction == Faction.OfPlayer) && Storage != null)
                    return true;
                return false;
            }
        }


        public ITab_VerticalStorage()
        {
            labelKey = "VerticalStorageContents";
            containedItemsKey = "ContainedItems";
            canRemoveThings = false;
        }
    }
}
