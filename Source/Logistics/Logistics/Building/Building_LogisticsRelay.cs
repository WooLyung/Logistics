using RimWorld;
using Verse;

namespace Logistics
{
    public class Building_LogisticsRelay : Building_ConveyorDevice, IStoreSettingsParent
    {
        private StorageSettings storageSettings;

        public override ConveyorDeviceType Type => ConveyorDeviceType.IO;
        public override ConveyorDeviceDir OutputDir => RotDir;
        public bool StorageTabVisible => true;

        public StorageSettings GetParentStoreSettings() => null;
        public StorageSettings GetStoreSettings() => storageSettings;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref storageSettings, "storageSettings", this);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (storageSettings == null)
                storageSettings = new StorageSettings(this);
        }

        public void Notify_SettingsChanged()
        {
        }

        public override void Tick()
        {
            base.Tick();

            if (this.IsHashIntervalTick(500) && this.IsOperational())
                Translate();
        }

        private void Translate()
        {
            Room to = null;
            foreach (var device in ConveyorSystem.GetOutputs(this))
            {
                if (device is Building_ConveyorPort port)
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
                if (device is Building_ConveyorPort port)
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

            foreach (Thing target in from.GetAllItemsInContainer())
            {
                Log.Message(target.def.defName);

                if (storageSettings.AllowedToAccept(target))
                    if (Translator.ToWarehouseAny(target, to))
                        return;
            }
        }
    }
}
