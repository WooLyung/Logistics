using Verse;
using System.Linq;

namespace Logistics
{
    public static class Translator
    {
        public static bool ToStorageAny(Thing thing, Room room, bool network = true)
        {
            int stackCount = thing.stackCount;
            int remained = thing.stackCount;

            foreach (IStorage storage in room.GetStorages(network))
                if (storage.TryInsert(thing, out remained))
                    if (remained == 0)
                        break;

            return stackCount != remained;
        }
    }
}
