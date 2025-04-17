using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse.AI.Group;
using Verse.AI;
using Verse;

namespace Logistics
{
    public class ITab_VerticalStorage : ITab_ContentsBase
    {
        public override IList<Thing> container => Storage.innerContainer;
        public Comp_VerticalStorage Storage => SelThing.TryGetComp<Comp_VerticalStorage>();

        public ITab_VerticalStorage()
        {
            canRemoveThings = false;
            labelKey = "TabTransporterContents";
            containedItemsKey = "ContainedItems";
        }
    }
}
