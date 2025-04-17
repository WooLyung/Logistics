using Verse;

namespace Logistics
{
    public class CompProperties_VerticalStorage : CompProperties
    {
        public CompProperties_VerticalStorage() => compClass = typeof(Comp_VerticalStorage);

        public int maxStack = 1;
    }
}
