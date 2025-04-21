using RimWorld;
using UnityEngine;
using Verse;

namespace Logistics
{
    public class Building_LogisticsRelay : Building_ConveyorDevice, IStoreSettingsParent
    {
        private StorageSettings storageSettings;

        public override ConveyorDeviceType DeviceType => ConveyorDeviceType.IO;
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
            Room to = null;
            foreach (var device in ConveyorSystem.GetOutputs(this))
            {
                if (device is Building_ConveyorPort port && port.IsActive())
                {
                    to = LogisticsSystem.GetAvailableSystemRoomWithConveyorPort(port);
                    if (to != null)
                        break;
                    return;
                }
            }
            if (to == null)
            {
                to = (Position + Rotation.FacingCell).GetRoom(Map);
                if (!LogisticsSystem.IsAvailableSystem(to))
                    return;
            }

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

            foreach (IStorage storage in from.GetStorages())
            {
                Thing target = storage.GetAnyStack(GetStoreSettings());
                if (target != null && Translator.ToStorageAny(target, to))
                    return;
            }
        }
    }
}
