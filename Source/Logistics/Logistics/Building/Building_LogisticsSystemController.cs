using RimWorld;
using Verse;

namespace Logistics
{
    public class Building_LogisticsSystemController : NetworkDevice
    {
        protected override string DefaultID => "Controller_" + Rand.Range(1000, 9999);

        public override void TickRare()
        {
            base.TickRare();

            CompPowerTrader comp = GetComp<CompPowerTrader>();
            Room room = this.GetRoom();

            if (comp != null)
            {
                if (!room.PsychologicallyOutdoors)
                {
                    int dynamicUsage = room == null ? 0 : room.CellCount * 20;
                    comp.PowerOutput = -dynamicUsage - 500;
                }
                else
                    comp.PowerOutput = -500;
            }
        }
    }
}
