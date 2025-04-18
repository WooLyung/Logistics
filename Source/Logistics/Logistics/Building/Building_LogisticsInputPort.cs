using Verse;

namespace Logistics
{
    public class Building_LogisticsInputPort : Building_ConveyorDevice
    {
        public override ConveyorDeviceType Type => ConveyorDeviceType.INPUT;
        public override ConveyorDeviceDir OutputDir => RotDir;

        public override void Tick()
        {
            base.Tick();

            if (this.IsHashIntervalTick(500) && this.IsActive())
                Translate();
        }

        private void Translate()
        {
            Room room = null;
            foreach (var device in ConveyorSystem.GetOutputs(this))
            {
                if (device is Building_ConveyorPort port && port.IsActive())
                {
                    room = LogisticsSystem.GetAvailableSystemRoomWithConveyorPort(port);
                    if (room != null)
                        break;
                    return;
                }
            }

            if (room == null)
            {
                room = (Position + Rotation.FacingCell).GetRoom(Map);
                if (!LogisticsSystem.IsAvailableSystem(room))
                    return;
            }

            var thingList = (Position - Rotation.FacingCell).GetThingList(Map);
            foreach (Thing thing in thingList)
                if (thing.def.EverStorable(true))
                    if (Translator.ToWarehouseAny(thing, room))
                        break;
        }
    }
}
