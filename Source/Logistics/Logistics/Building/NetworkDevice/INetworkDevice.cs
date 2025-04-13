namespace Logistics
{
    public interface INetworkDevice
    {
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
