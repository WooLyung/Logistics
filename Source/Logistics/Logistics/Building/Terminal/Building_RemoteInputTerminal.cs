namespace Logistics
{
    public class Building_RemoteInputTerminal : Building_RemoteTerminal
    {
        public override ConveyorDeviceType DeviceType => ConveyorDeviceType.Input;
        public override TerminalType TermType => TerminalType.Input;
    }
}
