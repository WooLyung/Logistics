namespace Logistics
{
    public class Building_IOTerminal : Building_Terminal
    {
        public override ConveyorDeviceType DeviceType => ConveyorDeviceType.IO;
        public override TerminalType TermType => TerminalType.IO;
    }
}
