using Verse;

namespace Logistics
{
    public abstract class Building_ConveyorDevice : Building
    {
        public enum DeviceType
        {
            NONE, INPUT, OUTPUT, IO
        };

        public enum Dir
        {
            NORTH, SOUTH, WEST, EAST, ALL
        }

        public virtual DeviceType Type { get; }
        public virtual Dir InputDir => Dir.ALL;
        public virtual Dir OutputDir => Dir.ALL;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (Type == DeviceType.INPUT)
                ConveyorSystem.AddInput(map, this, !respawningAfterLoad);
            if (Type == DeviceType.OUTPUT)
                ConveyorSystem.AddOutput(map, this, !respawningAfterLoad);
            if (Type == DeviceType.IO)
                ConveyorSystem.AddIO(map, this, !respawningAfterLoad);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            if (Type == DeviceType.INPUT)
                ConveyorSystem.RemoveInput(Map, this);
            if (Type == DeviceType.OUTPUT)
                ConveyorSystem.RemoveOutput(Map, this);
            if (Type == DeviceType.IO)
                ConveyorSystem.RemoveIO(Map, this);
            base.DeSpawn(mode);
        }
    }
}
