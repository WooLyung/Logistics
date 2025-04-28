using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Logistics
{
    public class Building_MatterCondenser : Building_ConveyorDevice
    {
        public override ConveyorDeviceType DeviceType => ConveyorDeviceType.Output;

        const int MaxProgress = 5000;
        private int progress = 0;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref progress, "progress", 0);
        }

        public override string GetInspectString()
        {
            string baseStr = base.GetInspectString();
            string extra = $"Progress: {progress}/{MaxProgress}";
            if (!baseStr.NullOrEmpty())
                baseStr += "\n";
            return baseStr + extra;
        }

        public override void Tick()
        {
            base.Tick();

            if (this.IsHashIntervalTick(2000) && this.IsActive() && progress < MaxProgress)
                Condense();
        }

        private void Condense()
        {
            if (!LogisticsSystem.IsAvailableSystem(this.GetRoom()))
                return;

            List<Thing> things = new List<Thing>();
            foreach (IStorage storage in this.GetRoom().GetActiveStorages())
                if (storage.IsActive)
                    foreach (Thing thing in storage.StoredThings)
                        things.Add(thing);

            if (!things.Empty())
            {
                Thing target = things.RandomElement();
                progress += target.stackCount;
                if (progress > MaxProgress)
                    progress = MaxProgress;
                target.TakeDamage(new DamageInfo(DamageDefOf.Crush, 1000000f));
            }
        }
    }
}
