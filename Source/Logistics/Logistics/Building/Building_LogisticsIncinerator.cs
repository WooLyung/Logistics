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

            if (this.IsHashIntervalTick(1000) && this.IsOperational())
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

            if (!things.Empty())
            {
                Thing target = things.RandomElement();
                target.TakeDamage(new DamageInfo(DamageDefOf.Flame, 1000f));
            }
        }
    }
}
