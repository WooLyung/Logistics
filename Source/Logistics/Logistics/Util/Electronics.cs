using RimWorld;
using Verse;

namespace Logistics.Util
{
    static class Electronics
    {
        public static bool IsOperational(this Thing thing)
        {
            if (thing == null || thing.Destroyed || !thing.Spawned || thing.IsBurning())
                return false;

            var flick = thing.TryGetComp<CompFlickable>();
            if (flick != null && !flick.SwitchIsOn)
                return false;

            var power = thing.TryGetComp<CompPowerTrader>();
            if (power != null && !power.PowerOn)
                return false;

            var breakdown = thing.TryGetComp<CompBreakdownable>();
            if (breakdown != null && breakdown.BrokenDown)
                return false;

            return true;
        }
    }
}
