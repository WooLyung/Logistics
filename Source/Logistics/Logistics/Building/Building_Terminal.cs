using Verse;

namespace Logistics
{
    public class Building_Terminal : Building_ConveyorDevice
    {
        public override DeviceType Type
        {
            get
            {
                if (GetComp<Comp_InputTerminal>() != null)
                {
                    if (GetComp<Comp_OutputTerminal>() != null)
                        return DeviceType.IO;
                    return DeviceType.INPUT;
                }
                if (GetComp<Comp_OutputTerminal>() != null)
                    return DeviceType.OUTPUT;
                return DeviceType.NONE;
            }
        }
    }
}
