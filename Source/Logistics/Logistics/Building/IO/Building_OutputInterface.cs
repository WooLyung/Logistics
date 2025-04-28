using RimWorld;
using Verse;

namespace Logistics
{
    public class Building_OutputInterface : Building_ConveyorDevice, IStoreSettingsParent
    {
        private StorageSettings storageSettings;

        public override ConveyorDeviceType DeviceType => ConveyorDeviceType.Output;
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
            Room from = LogisticsSystem.GetAvailableBackwardWarehouse(this, ConveyorDeviceType.Input);
            if (from == null)
                return;

            foreach (Thing target in (Position + Rotation.FacingCell).GetThingList(Map))
            {
                if (target.HasComp<CompRefuelable>())
                {
                    CompRefuelable refuelable = target.TryGetComp<CompRefuelable>();
                    CompProperties_Refuelable props = refuelable.props as CompProperties_Refuelable;

                    if (props != null && (int)(props.fuelCapacity - refuelable.Fuel) > 0)
                    {
                        int need = refuelable.GetFuelCountToFullyRefuel();
                        foreach (IStorage storage in from.GetActiveStorages())
                        {
                            int consume = storage.TryConsume(props.fuelFilter, storageSettings, need);
                            if (consume > 0)
                            {
                                refuelable.Refuel(consume);
                                return;
                            }
                        }
                    }
                }

                if (target is ThingWithComps target2)
                {
                    foreach (ThingComp comp in target2.AllComps)
                        if (comp is IComp_OutputCompotable compotable)
                            if (compotable.TryInsert(from))
                                return;
                }
            }
        }
    }
}
