using Verse;

namespace Logistics
{
    public class Building_Terminal : Building_ConveyorDevice, ITerminal
    {
        public override ConveyorDeviceType Type
        {
            get
            {
                if (GetComp<Comp_InputTerminal>() != null)
                {
                    if (GetComp<Comp_OutputTerminal>() != null)
                        return ConveyorDeviceType.IO;
                    return ConveyorDeviceType.INPUT;
                }
                if (GetComp<Comp_OutputTerminal>() != null)
                    return ConveyorDeviceType.OUTPUT;
                return ConveyorDeviceType.NONE;
            }
        }

        TerminalType ITerminal.Type => TerminalType.IO;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            LCache.GetLCache(map).AddTerminal(this);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            LCache.GetLCache(Map).AddTerminal(this);
            base.DeSpawn(mode);
        }
    }
}
