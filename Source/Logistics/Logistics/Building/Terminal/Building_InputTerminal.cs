namespace Logistics
{
    public class Building_InputTerminal : Building_Terminal
    {
        public override ConveyorDeviceType DeviceType => ConveyorDeviceType.Input;
        public override TerminalType TermType => TerminalType.Input;
    }
}
