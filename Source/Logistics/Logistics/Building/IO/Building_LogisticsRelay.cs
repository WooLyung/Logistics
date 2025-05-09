﻿using RimWorld;
using Verse;

namespace Logistics
{
    public class Building_LogisticsRelay : Building_ConveyorDevice, IStoreSettingsParent
    {
        private StorageSettings storageSettings;

        public override ConveyorDeviceType DeviceType => ConveyorDeviceType.IO;
        public override ConveyorDeviceDir InputDir => RotDir;
        public override ConveyorDeviceDir OutputDir => RotDir;
        public bool StorageTabVisible => true;

        public StorageSettings GetParentStoreSettings()
        {
            StorageSettings fixedStorageSettings = def.building.fixedStorageSettings;
            if (fixedStorageSettings != null)
                return fixedStorageSettings;
            return StorageSettings.EverStorableFixedSettings();
        }
        public StorageSettings GetStoreSettings() => storageSettings;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (storageSettings == null)
                storageSettings = new StorageSettings(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref storageSettings, "storageSettings", this);
        }

        public void Notify_SettingsChanged()
        {
        }

        public override void Tick()
        {
            base.Tick();

            if (this.IsHashIntervalTick(500) && this.IsActive())
                Translate();
        }

        private void Translate()
        {
            Room to = LogisticsSystem.GetAvailableForwardWarehouse(this, ConveyorDeviceType.Output);
            Room from = LogisticsSystem.GetAvailableBackwardWarehouse(this, ConveyorDeviceType.Input);

            if (to == null || from == null)
                return;

            foreach (IStorage storage in from.GetActiveStorages())
            {
                Thing target = storage.GetAnyStack(null, GetStoreSettings());
                if (target != null && Translator.ToStorageAny(target, to))
                    return;
            }
        }
    }
}
