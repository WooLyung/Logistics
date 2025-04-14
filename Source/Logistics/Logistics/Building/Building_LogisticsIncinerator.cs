using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace Logistics
{
    public class Building_LogisticsIncinerator : Building
    {
        public override void Tick()
        {
            base.Tick();

            if (this.IsHashIntervalTick(1000))
                Incinerate();
        }

        private void Incinerate()
        {
            if (!LogisticsSystem.IsAvailableSystem(this.GetRoom()))
                return;

            List<Thing> things = new List<Thing>();
            foreach (Thing thing in this.GetRoom().ContainedAndAdjacentThings)
                if (thing.IsInContainer())
                    things.Add(thing);

            Thing target = things.RandomElement();
            if (target != null)
                target.Destroy(DestroyMode.Deconstruct);
        }
    }
}
