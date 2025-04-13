using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Logistics
{
    public class Building_RemoteTerminal : Building_Terminal, INetworkDevice
    {
        private string networkID;

        public string DefaultID => "Terminal_" + Rand.Range(1000, 9999);

        public string NetworkID
        {
            get => networkID;
            set => networkID = value;
        }

        public Building_RemoteTerminal()
        {
            networkID = DefaultID;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref networkID, "NetworkID", DefaultID);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos()) yield return g;

            yield return new Command_Action
            {
                defaultLabel = "NetworkIDLabel".Translate(),
                defaultDesc = "NetworkIDDesc".Translate(),
                icon = TexButton.Rename,
                action = () => {
                    Find.WindowStack.Add(new Dialog_RenameController(this));
                }
            };
        }

        public override string GetInspectString()
        {
            StringBuilder sb = new StringBuilder();

            string baseStr = base.GetInspectString();
            if (!baseStr.NullOrEmpty())
                sb.AppendLine(baseStr);

            sb.AppendLine($"{"NetworkID".Translate()}: {networkID}");
            return sb.ToString().TrimEndNewlines();
        }
    }
}
