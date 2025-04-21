using Verse;

namespace Logistics
{
    public enum ConveyorDeviceType
    {
        None, Input, Output, IO
    };

    public enum ConveyorDeviceDir
    {
        North, South, West, East, All
    }

    public interface IConveyorDevice
    {
        Thing Thing { get; }
        ConveyorDeviceType DeviceType { get; }
        ConveyorDeviceDir InputDir { get; }
        ConveyorDeviceDir OutputDir { get; }
    }
}
