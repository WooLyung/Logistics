namespace Logistics
{
    public interface NetworkDevice
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
