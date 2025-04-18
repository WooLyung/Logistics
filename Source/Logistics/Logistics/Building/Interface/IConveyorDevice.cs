using Verse;

namespace Logistics
{
    public enum ConveyorDeviceType
    {
        NONE, INPUT, OUTPUT, IO
    };

    public enum ConveyorDeviceDir
    {
        NORTH, SOUTH, WEST, EAST, ALL
    }

    public interface IConveyorDevice
    {
        Thing Thing { get; }
        ConveyorDeviceType Type { get; }
        ConveyorDeviceDir InputDir { get; }
        ConveyorDeviceDir OutputDir { get; }
    }
}
