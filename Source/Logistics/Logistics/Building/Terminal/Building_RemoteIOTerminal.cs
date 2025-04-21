namespace Logistics
{
    public class Building_RemoteIOTerminal : Building_RemoteTerminal
    {
        public override ConveyorDeviceType DeviceType => ConveyorDeviceType.IO;
        public override TerminalType TermType => TerminalType.IO;
    }
}
