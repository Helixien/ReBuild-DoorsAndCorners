using RimWorld;
using Verse;

namespace ReBuildDoorsAndCorners
{
    public class Building_MultiTileDoorStuffColor : Building_MultiTileDoor
    {
        public Graphic upperMoverGraphic;

        public Graphic UpperMoverGraphic
        {
            get
            {
                if (upperMoverGraphic == null)
                {
                    Graphic graphic = def.building.upperMoverGraphic?.Graphic;
                    if (graphic == null)
                    {
                        return null;
                    }
                    upperMoverGraphic = graphic.GetColoredVersion(graphic.Shader, DrawColor, Graphic.ColorTwo);
                }
                return upperMoverGraphic;
            }
        }
    }
}
