using Verse;
using System.Linq;

namespace Logistics
{
    public static class Translator
    {
        public static bool ToStorageAny(Thing thing, Room room)
        {
            int stackCount = thing.stackCount;
            int remained = thing.stackCount;

            foreach (IStorage storage in room.GetStorages())
                if (storage.TryInsert(thing, out remained))
                    if (remained == 0)
                        break;

            return stackCount != remained;
        }
    }
}
