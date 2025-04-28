using Verse;

namespace Logistics
{
    public class Building_LogisticsInputPort : Building_ConveyorDevice
    {
        public override ConveyorDeviceType DeviceType => ConveyorDeviceType.Input;
        public override ConveyorDeviceDir InputDir => RotDir;

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

            var thingList = (Position - Rotation.FacingCell).GetThingList(Map);
            foreach (Thing thing in thingList)
                if (thing.def.EverHaulable)
                    if (Translator.ToStorageAny(thing, to))
                        return;

            thingList = (Position - Rotation.FacingCell).GetThingList(Map);
            foreach (Thing _thing in thingList)
                if (_thing is IStorage storage && storage.IsActive)
                    foreach (Thing thing in storage.StoredThings)
                        if (Translator.ToStorageAny(thing, to))
                            return;
        }
    }
}
