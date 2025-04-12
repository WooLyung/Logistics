using RimWorld;
using UnityEngine;
using Verse;

namespace Logistics
{
    public class Dialog_RenameController : Window
    {
        private string curName;
        private readonly NetworkDevice device;

        public override Vector2 InitialSize => new Vector2(400f, 150f);

        public Dialog_RenameController(NetworkDevice device)
        {
            this.device = device;
            curName = device.NetworkID;
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(0f, 0f, inRect.width, 30f), "NetworkIDEnter".Translate());

            curName = Widgets.TextField(new Rect(0f, 40f, inRect.width, 30f), curName);

            if (Widgets.ButtonText(new Rect(0f, 80f, 120f, 30f), "NetworkIDConfirm".Translate()))
            {
                device.NetworkID = curName;
                Messages.Message("NetworkIDMessage".Translate(), MessageTypeDefOf.NeutralEvent);
                Close();
            }

            if (Widgets.ButtonText(new Rect(inRect.width - 120f, 80f, 120f, 30f), "NetworkIDCancel".Translate()))
                Close();
        }
    }
}
