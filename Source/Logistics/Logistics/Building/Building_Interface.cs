using Verse;

namespace Logistics
{
    public class Building_Interface : Building_ConveyorDevice
    {
        public override DeviceType Type
        {
            get
            {
                if (GetComp<Comp_InputInterface>() != null)
                {
                    if (GetComp<Comp_OutputInterface>() != null)
                        return DeviceType.IO;
                    return DeviceType.INPUT;
                }
                if (GetComp<Comp_OutputInterface>() != null)
                    return DeviceType.OUTPUT;
                return DeviceType.NONE;
            }
        }
    }
}
