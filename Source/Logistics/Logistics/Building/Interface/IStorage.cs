using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Logistics
{
    public interface IStorage
    {
        Thing Thing { get; }

        Thing GetAnyStack(StorageSettings filter);
        bool TryInsert(Thing thing, out int remained);
        bool HasThing(Thing thing);

        IEnumerable<Thing> StoredThings { get; }
    }
}
