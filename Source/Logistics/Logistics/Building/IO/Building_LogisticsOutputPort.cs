using RimWorld;
using Verse;
using Verse.Noise;

namespace Logistics
{
    public class Building_LogisticsOutputPort : Building_ConveyorDevice, IStoreSettingsParent
    {
        private StorageSettings storageSettings;

        public override ConveyorDeviceType DeviceType => ConveyorDeviceType.Output;
        public override ConveyorDeviceDir OutputDir => RotDir;
        public bool StorageTabVisible => true;
        StorageSettings IStoreSettingsParent.GetStoreSettings() => storageSettings;
        public StorageSettings GetParentStoreSettings()
        {
            StorageSettings fixedStorageSettings = def.building.fixedStorageSettings;
            if (fixedStorageSettings != null)
                return fixedStorageSettings;
            return StorageSettings.EverStorableFixedSettings();
        }

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

        public override void Tick()
        {
            base.Tick();

            if (this.IsHashIntervalTick(500) && this.IsActive())
                Translate();
        }

        public void Notify_SettingsChanged()
        {
        }

        private void Translate()
        {
            Room from = LogisticsSystem.GetAvailableBackwardWarehouse(this, ConveyorDeviceType.Input);
            if (from == null)
                return;

            var cell = Position + Rotation.FacingCell;
            var thingList = cell.GetThingList(Map);
            foreach (IStorage storage in from.GetActiveStorages())
                foreach (Thing item in storage.StoredThings)
                    foreach (Thing thing in thingList)
                        if (thing is IStorage storage0 && storage0.TryInsert(item, out _))
                            return;

            thingList = cell.GetThingList(Map);
            foreach (IStorage storage in from.GetActiveStorages())
            {
                foreach (Thing item in storage.StoredThings)
                {
                    if (!storageSettings.AllowedToAccept(item))
                        continue;

                    int stackCount = item.stackCount;
                    int itemCount = 0;
                    foreach (Thing thing in thingList)
                    {
                        if (thing.def.EverHaulable)
                            itemCount++;
                        if (thing.CanStackWith(item))
                        {
                            int space = thing.def.stackLimit - thing.stackCount;
                            if (space >= item.stackCount)
                            {
                                thing.TryAbsorbStack(item, true);
                                return;
                            }
                            else if (space > 0)
                                thing.TryAbsorbStack(item.SplitOff(space), true);
                        }
                    }
                    if (itemCount < cell.GetMaxItemsAllowedInCell(Map))
                    {
                        if (item.Spawned)
                            item.DeSpawn();
                        GenPlace.TryPlaceThing(item, cell, Map, ThingPlaceMode.Direct);
                        return;
                    }

                    if (stackCount != item.stackCount)
                        return;
                }
            }
        }
    }
}
