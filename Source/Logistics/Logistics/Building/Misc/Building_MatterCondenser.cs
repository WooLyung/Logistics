using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Logistics
{
    public class Building_MatterCondenser : Building_ConveyorDevice, IStoreSettingsParent
    {
        public override ConveyorDeviceType DeviceType => ConveyorDeviceType.Output;

        const int MaxProgress = 5000;
        private int progress = 0;

        private StorageSettings storageSettings;
        public bool StorageTabVisible => true;
        StorageSettings IStoreSettingsParent.GetStoreSettings() => storageSettings;

        public bool CanExtract() => progress >= MaxProgress;

        public void Flush() => progress = 0;

        public StorageSettings GetParentStoreSettings()
        {
            StorageSettings fixedStorageSettings = def.building.fixedStorageSettings;
            if (fixedStorageSettings != null)
                return fixedStorageSettings;
            return StorageSettings.EverStorableFixedSettings();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref storageSettings, "storageSettings", this);
            Scribe_Values.Look(ref progress, "progress", 0);
        }

        public void Notify_SettingsChanged()
        {
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (storageSettings == null)
                storageSettings = new StorageSettings(this);
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
                        if (storageSettings.AllowedToAccept(thing))
                            things.Add(thing);

            if (!things.Empty())
            {
                Thing target = things.RandomElement();
                progress += target.stackCount;
                if (progress > MaxProgress)
                    progress = MaxProgress;
                target.Destroy();
            }
        }
    }
}
