using RimWorld;
using Verse;

namespace Logistics
{
    public abstract class Building_ConveyorDevice : Building, IConveyorDevice
    {
        public virtual ConveyorDeviceType DeviceType { get; }
        public virtual ConveyorDeviceDir InputDir => ConveyorDeviceDir.All;
        public virtual ConveyorDeviceDir OutputDir => ConveyorDeviceDir.All;

        protected ConveyorDeviceDir RotDir
        {
            get
            {
                if (Rotation == Rot4.North)
                    return ConveyorDeviceDir.North;
                if (Rotation == Rot4.South)
                    return ConveyorDeviceDir.South;
                if (Rotation == Rot4.East)
                    return ConveyorDeviceDir.East;
                if (Rotation == Rot4.West)
                    return ConveyorDeviceDir.West;
                return ConveyorDeviceDir.All;
            }
        }

        public Thing Thing => this;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (DeviceType == ConveyorDeviceType.Input)
                ConveyorSystem.AddInput(map, this, !respawningAfterLoad);
            if (DeviceType == ConveyorDeviceType.Output)
                ConveyorSystem.AddOutput(map, this, !respawningAfterLoad);
            if (DeviceType == ConveyorDeviceType.IO)
                ConveyorSystem.AddIO(map, this, !respawningAfterLoad);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            if (DeviceType == ConveyorDeviceType.Input)
                ConveyorSystem.RemoveInput(Map, this);
            if (DeviceType == ConveyorDeviceType.Output)
                ConveyorSystem.RemoveOutput(Map, this);
            if (DeviceType == ConveyorDeviceType.IO)
                ConveyorSystem.RemoveIO(Map, this);
            base.DeSpawn(mode);
        }
    }
}
