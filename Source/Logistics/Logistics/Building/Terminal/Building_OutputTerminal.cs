namespace Logistics
{
    public class Building_OutputTerminal : Building_Terminal
    {
        public override ConveyorDeviceType DeviceType => ConveyorDeviceType.Output;
        public override TerminalType TermType => TerminalType.Output;
    }
}
