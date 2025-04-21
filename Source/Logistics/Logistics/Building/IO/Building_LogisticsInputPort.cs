using Verse;

namespace Logistics
{
    public class Building_LogisticsInputPort : Building_ConveyorDevice
    {
        public override ConveyorDeviceType DeviceType => ConveyorDeviceType.Input;

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
