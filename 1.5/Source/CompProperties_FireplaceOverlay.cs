using RimWorld;
using UnityEngine;
using VanillaFurnitureExpanded;
using Verse;

namespace ReBuildDoorsAndCorners
{
    public class CompProperties_FireplaceOverlay : CompProperties
    {
        public GraphicData graphicData;

        public CompProperties_FireplaceOverlay()
        {
            this.compClass = typeof(CompFireplaceOverlay);
        }
    }

    public class CompFireplaceOverlay : ThingComp
    {
        public CompRefuelable compRefuelable;
        public Graphic cachedGraphic;
        public Graphic Graphic => cachedGraphic ??= Props.graphicData.GraphicColoredFor(parent);
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            compRefuelable = parent.GetComp<CompRefuelable>();
        }
        public CompProperties_FireplaceOverlay Props => base.props as CompProperties_FireplaceOverlay;
        public override void PostDraw()
        {
            base.PostDraw();
            if (compRefuelable.HasFuel)
            {
                Graphic.Draw(parent.DrawPos + new Vector3(0, 0.001f, 0), parent.Rotation, parent);
            }
        }
    }
}
