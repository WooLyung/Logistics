using RimWorld;
using Verse;

namespace Logistics
{
    public class Building_OutputInterface : Building_ConveyorDevice, IStoreSettingsParent
    {
        private StorageSettings storageSettings;

        public override ConveyorDeviceType DeviceType => ConveyorDeviceType.Output;
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
            Room from = null;
            foreach (var device in ConveyorSystem.GetInputs(this))
            {
                if (device is Building_ConveyorPort port && port.IsActive())
                {
                    from = LogisticsSystem.GetAvailableSystemRoomWithConveyorPort(port);
                    if (from != null)
                        break;
                    return;
                }
            }

            if (from == null)
            {
                from = (Position - Rotation.FacingCell).GetRoom(Map);
                if (!LogisticsSystem.IsAvailableSystem(from))
                    return;
            }

            foreach (Thing target in (Position + Rotation.FacingCell).GetThingList(Map))
            {
                if (target.HasComp<CompRefuelable>())
                {
                    CompRefuelable refuelable = target.TryGetComp<CompRefuelable>();
                    CompProperties_Refuelable props = refuelable.props as CompProperties_Refuelable;

                    if (props != null && (int)(props.fuelCapacity - refuelable.Fuel) > 0)
                    {
                        int need = (int)(props.fuelCapacity - refuelable.Fuel);
                        foreach (IStorage storage in from.GetActiveStorages())
                        {
                            Thing fuel = storage.GetAnyStack(props.fuelFilter, storageSettings);
                            if (fuel != null)
                            {
                                if (fuel.stackCount >= need)
                                {
                                    refuelable.Refuel(need);
                                    fuel.SplitOff(need).Destroy();
                                    break;
                                }
                                else
                                {
                                    refuelable.Refuel(fuel.stackCount);
                                    fuel.Destroy();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
