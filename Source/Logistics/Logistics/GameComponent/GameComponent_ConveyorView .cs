using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Logistics
{
    public class GameComponent_ConveyorView : GameComponent
    {
        private static GameComponent_ConveyorView singleton = null;

        public static bool ShowConveyors
        {
            get
            {
                if (singleton == null)
                    singleton = Current.Game?.GetComponent<GameComponent_ConveyorView>();
                return singleton?.showConveyors ?? true;
            }

            set
            {
                if (singleton == null)
                    singleton = Current.Game?.GetComponent<GameComponent_ConveyorView>();
                if (singleton != null)
                    singleton.showConveyors = value;
            }
        }

        public bool showConveyors = true;

        public GameComponent_ConveyorView(Game game) : base()
        {
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref showConveyors, "showConveyors", true);
        }
    }
}
