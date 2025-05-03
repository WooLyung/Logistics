using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace Logistics
{
    public class Building_LogisticsNetworkLinker : Building, INetworkLinker
    {
        public Thing Thing => this;

        public string LinkTargetID
        {
            get => target;
            set => target = value;
        }

        public class Dialog_RenameController : Window
        {
            private string curName;
            private readonly Building_LogisticsNetworkLinker linker;

            public override Vector2 InitialSize => new Vector2(400f, 150f);

            public Dialog_RenameController(Building_LogisticsNetworkLinker linker)
            {
                this.linker = linker;
                curName = linker.LinkTargetID;
                forcePause = true;
                absorbInputAroundWindow = true;
                closeOnClickedOutside = true;
            }

            public override void DoWindowContents(Rect inRect)
            {
                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(0f, 0f, inRect.width, 30f), "LinkTargetEnter".Translate());

                curName = Widgets.TextField(new Rect(0f, 40f, inRect.width, 30f), curName);

                if (Widgets.ButtonText(new Rect(0f, 80f, 120f, 30f), "LinkTargetConfirm".Translate()))
                {
                    linker.LinkTargetID = curName;
                    Messages.Message("LinkTargetMessage".Translate(), MessageTypeDefOf.NeutralEvent);
                    Close();
                }

                if (Widgets.ButtonText(new Rect(inRect.width - 120f, 80f, 120f, 30f), "LinkTargetCancel".Translate()))
                    Close();
            }
        }

        private string target = "None";
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref target, "LinkTargetID", "None");
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos()) yield return g;

            yield return new Command_Action
            {
                defaultLabel = "LinkTargetLabel".Translate(),
                defaultDesc = "LinkTargetDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("Things/Building/Misc/LogisticsSystemController"),
                action = () =>
                {
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

            sb.AppendLine($"{"LinkTargetID".Translate()}: {LinkTargetID}");
            return sb.ToString().TrimEndNewlines();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            LCache.GetLCache(map).AddNetworkLinker(this);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            LCache.GetLCache(Map).RemoveNetworkLinker(this);
            base.DeSpawn(mode);
        }
    }
}
