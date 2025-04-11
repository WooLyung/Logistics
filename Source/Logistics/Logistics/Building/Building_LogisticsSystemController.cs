using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace Logistics
{
    public class Building_LogisticsSystemController : Building
    {
        public class Dialog_RenameController : Window
        {
            private string curName;
            private readonly Building_LogisticsSystemController controller;

            public override Vector2 InitialSize => new Vector2(400f, 150f);

            public Dialog_RenameController(Building_LogisticsSystemController controller)
            {
                this.controller = controller;
                curName = controller.ControllerID;
                forcePause = true;
                absorbInputAroundWindow = true;
                closeOnClickedOutside = true;
            }

            public override void DoWindowContents(Rect inRect)
            {
                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(0f, 0f, inRect.width, 30f), "ControllerRenameEnter".Translate());

                curName = Widgets.TextField(new Rect(0f, 40f, inRect.width, 30f), curName);

                if (Widgets.ButtonText(new Rect(0f, 80f, 120f, 30f), "ControllerRenameConfirm".Translate()))
                {
                    controller.ControllerID = curName;
                    Messages.Message("ControllerRenameMessage".Translate(), MessageTypeDefOf.NeutralEvent);
                    Close();
                }

                if (Widgets.ButtonText(new Rect(inRect.width - 120f, 80f, 120f, 30f), "ControllerRenameCancel".Translate()))
                    Close();
            }
        }

        private string controllerID = "Controller_" + Rand.Range(1000, 9999);

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

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref controllerID, "controllerID", null);

            if (Scribe.mode == LoadSaveMode.PostLoadInit && controllerID == null)
                controllerID = "Controller_" + Rand.Range(1000, 9999);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos()) yield return g;

            yield return new Command_Action
            {
                defaultLabel = "ControllerRenameLabel".Translate(),
                defaultDesc = "ControllerRenameDesc".Translate(),
                icon = TexButton.Rename,
                action = () => {
                    Find.WindowStack.Add(new Dialog_RenameController(this));
                }
            };
        }

        public string ControllerID
        {
            get => controllerID;
            set => controllerID = value;
        }

        public override string GetInspectString()
        {
            StringBuilder sb = new StringBuilder();

            string baseStr = base.GetInspectString();
            if (!baseStr.NullOrEmpty())
                sb.AppendLine(baseStr);

            sb.AppendLine($"{"ControllerID".Translate()}: {controllerID}");
            return sb.ToString().TrimEndNewlines();
        }
    }
}
