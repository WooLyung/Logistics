namespace Logistics
{
    public class Building_RemoteOutputTerminal : Building_RemoteTerminal
    {
        public override ConveyorDeviceType DeviceType => ConveyorDeviceType.Output;
        public override TerminalType TermType => TerminalType.Output;
    }
}
