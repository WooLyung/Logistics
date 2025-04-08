using Verse;

namespace Logistics
{
    public class CompProperties_InputInterface : CompProperties
    {
        public CompProperties_InputInterface() => compClass = typeof(Comp_InputInterface);

        public int inputTick = 60;
    }
}
