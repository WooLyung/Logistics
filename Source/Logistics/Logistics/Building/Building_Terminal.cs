using Verse;

namespace Logistics
{
    public class Building_Terminal : Building_ConveyorDevice
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
    }
}
