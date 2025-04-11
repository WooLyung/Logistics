using UnityEngine;
using Verse;

namespace Logistics
{
    public class Building_Conveyor : Building
    {
        public class Designator_ToggleConveyor : Designator
        {
            public Designator_ToggleConveyor()
            {
                defaultLabel = "ConveyorViewToggleLabel".Translate();
                defaultDesc = "ConveyorViewToggleDesc".Translate();
                if (GameComponent_ConveyorView.ShowConveyors)
                    icon = ContentFinder<Texture2D>.Get("UI/ConveyorOn");
                else
                    icon = ContentFinder<Texture2D>.Get("UI/ConveyorOff");
            }

            public override AcceptanceReport CanDesignateCell(IntVec3 c) => false;

            public override void DesignateSingleCell(IntVec3 c) { }

            public override void Selected()
            {
                Find.DesignatorManager.Deselect();
                GameComponent_ConveyorView.ShowConveyors = !GameComponent_ConveyorView.ShowConveyors;
                if (GameComponent_ConveyorView.ShowConveyors)
                    icon = ContentFinder<Texture2D>.Get("UI/ConveyorOn");
                else
                    icon = ContentFinder<Texture2D>.Get("UI/ConveyorOff");
            }
        }

        [StaticConstructorOnStartup]
        public static class ConveyorViewToggle
        {
            static ConveyorViewToggle()
            {
                var logisticsCat = DefDatabase<DesignationCategoryDef>.GetNamed("LogisticsCategory");
                logisticsCat.AllResolvedDesignators.Add(new Designator_ToggleConveyor());
            }
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            if (GameComponent_ConveyorView.ShowConveyors)
                base.DrawAt(drawLoc, flip);
        }
    }
}
