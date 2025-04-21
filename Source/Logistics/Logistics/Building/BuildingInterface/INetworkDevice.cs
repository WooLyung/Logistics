using Verse;

namespace Logistics
{
    public interface INetworkDevice
    {
        Thing Thing { get; }

        string DefaultID {
            get;
        }

        string NetworkID
        {
            get;
            set;
        }
    }
}
