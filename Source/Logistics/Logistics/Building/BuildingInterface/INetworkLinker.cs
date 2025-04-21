using Verse;

namespace Logistics
{
    public interface INetworkLinker
    {
        Thing Thing { get; }
        string LinkTargetID { get; set; }
    }
}
