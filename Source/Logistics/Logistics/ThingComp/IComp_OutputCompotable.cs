using Verse;

namespace Logistics
{
    public interface IComp_OutputCompotable
    {
        bool TryInsert(Room room, bool network = true);
    }
}
