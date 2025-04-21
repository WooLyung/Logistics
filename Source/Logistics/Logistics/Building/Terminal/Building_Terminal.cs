using Verse;

namespace Logistics
{
    abstract public class Building_Terminal : Building_ConveyorDevice, ITerminal
    {
        public virtual TerminalType TermType { get; }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            LCache.GetLCache(map).AddTerminal(this);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            LCache.GetLCache(Map).RemoveTerminal(this);
            base.DeSpawn(mode);
        }
    }
}
