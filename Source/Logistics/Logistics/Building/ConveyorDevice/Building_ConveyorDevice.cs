using RimWorld;
using Verse;

namespace Logistics
{
    public abstract class Building_ConveyorDevice : Building, IConveyorDevice
    {
        public virtual ConveyorDeviceType Type { get; }
        public virtual ConveyorDeviceDir InputDir => ConveyorDeviceDir.ALL;
        public virtual ConveyorDeviceDir OutputDir => ConveyorDeviceDir.ALL;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (Type == ConveyorDeviceType.INPUT)
                ConveyorSystem.AddInput(map, this, !respawningAfterLoad);
            if (Type == ConveyorDeviceType.OUTPUT)
                ConveyorSystem.AddOutput(map, this, !respawningAfterLoad);
            if (Type == ConveyorDeviceType.IO)
                ConveyorSystem.AddIO(map, this, !respawningAfterLoad);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            if (Type == ConveyorDeviceType.INPUT)
                ConveyorSystem.RemoveInput(Map, this);
            if (Type == ConveyorDeviceType.OUTPUT)
                ConveyorSystem.RemoveOutput(Map, this);
            if (Type == ConveyorDeviceType.IO)
                ConveyorSystem.RemoveIO(Map, this);
            base.DeSpawn(mode);
        }
    }
}
