using Verse;

namespace Logistics
{
    public class CompProperties_Conveyor : CompProperties
    {
        public CompProperties_Conveyor() => compClass = typeof(Comp_Conveyor);

        public int inputTick = 60;
    }
}
