using RimWorld;
using Verse;

namespace Logistics
{
    public class Building_InputInterface : Building_ConveyorDevice, IStoreSettingsParent
    {
        private StorageSettings storageSettings;

        public override ConveyorDeviceType DeviceType => ConveyorDeviceType.Input;
        public override ConveyorDeviceDir InputDir => RotDir;
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
            if (to == null)
                return;

            foreach (Thing target in (Position + Rotation.FacingCell).GetThingList(Map))
            {
                if (target is ThingWithComps target2)
                {
                    foreach (ThingComp comp in target2.AllComps)
                        if (comp is IComp_InputCompotable compotable)
                            if (compotable.TryExtract(to))
                                return;
                }
            }
        }
    }
}
