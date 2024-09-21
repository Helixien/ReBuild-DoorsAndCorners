using Verse;

namespace ReBuildDoorsAndCorners
{
    public class Building_CellWall : Building
    {
        private Graphic topRightGraphic;
        private Graphic bottomLeftGraphic;
        public override Graphic Graphic
        {
            get
            {
                var baseGraphic = base.Graphic;
                if (Spawned)
                {
                    if (HasWall(Rot4.North) || HasWall(Rot4.East))
                    {
                        if (topRightGraphic is null)
                        {
                            var copy = new GraphicData();
                            copy.CopyFrom(def.graphicData);
                            copy.texPath += "_TopRightConnector";
                            topRightGraphic = copy.GraphicColoredFor(this);
                        }
                        return topRightGraphic;
                    }
                    else if (HasWall(Rot4.South) || HasWall(Rot4.West))
                    {
                        if (bottomLeftGraphic is null)
                        {
                            var copy = new GraphicData();
                            copy.CopyFrom(def.graphicData);
                            copy.texPath += "_BottomLeftConnector";
                            bottomLeftGraphic = copy.GraphicColoredFor(this);
                        }
                        return bottomLeftGraphic;
                    }
                }
                return baseGraphic;
            }
        }

        private bool HasWall(Rot4 rot)
        {
            var edifice = (rot.FacingCell + Position).GetEdifice(Map);
            if (edifice is null || edifice.def == this.def)
            {
                return false;
            }
            return edifice.def.Fillage == FillCategory.Full;
        }
    }
}
