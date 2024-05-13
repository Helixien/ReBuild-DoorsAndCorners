using UnityEngine;
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
        public CompGlower glower;
        public Graphic cachedGraphic;
        public Graphic Graphic => cachedGraphic ??= Props.graphicData.GraphicColoredFor(parent);
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            glower = parent.GetComp<CompGlower>();
        }
        public CompProperties_FireplaceOverlay Props => base.props as CompProperties_FireplaceOverlay;
        public override void PostDraw()
        {
            base.PostDraw();
            if (glower.Glows)
            {
                Graphic.Draw(parent.DrawPos + new Vector3(0, 1, 0), parent.Rotation, parent);
            }
        }
    }
}
