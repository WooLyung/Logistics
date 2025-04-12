using RimWorld;
using Verse;

namespace Logistics
{
    public class Building_RemoteInterface : NetworkDevice
    {
        protected override string DefaultID => "Interface_" + Rand.Range(1000, 9999);
    }
}
