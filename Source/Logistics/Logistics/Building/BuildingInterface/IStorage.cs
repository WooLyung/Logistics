using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Logistics
{
    public interface IStorage
    {
        Thing Thing { get; }

        Thing GetAnyStack(ThingFilter filter1, StorageSettings filter2);
        int TryConsume(ThingFilter filter1, StorageSettings filter2, int max);

        bool IsActive { get; }
        bool TryInsert(Thing thing, out int remained);
        bool HasThing(Thing thing);

        IEnumerable<Thing> StoredThings { get; }
    }
}
