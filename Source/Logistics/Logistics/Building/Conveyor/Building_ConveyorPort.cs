using RimWorld;
using Verse;

namespace Logistics
{
    public class Building_ConveyorPort : Building_ConveyorDevice, IConveyorPort
    {
        public override ConveyorDeviceType DeviceType => ConveyorDeviceType.IO;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            LCache.GetLCache(map).AddConveyorPort(this);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            LCache.GetLCache(Map).RemoveConveyorPort(this);
            base.DeSpawn(mode);
        }
    }
}
