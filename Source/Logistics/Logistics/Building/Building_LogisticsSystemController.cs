using RimWorld;
using UnityEngine;
using Verse;

namespace Logistics
{
    public class Building_LogisticsSystemController : Building
    {
        public override void TickRare()
        {
            base.TickRare();

            CompPowerTrader comp = this.GetComp<CompPowerTrader>();
            if (comp != null)
            {
                Room room = this.GetRoom();
                int dynamicUsage = room == null ? 0 : room.CellCount * 10;
                comp.PowerOutput = -dynamicUsage - 500;
            }
        }
    }
}
