using System;
using System.Collections.Generic;
using Verse;

namespace ReBuildDoorsAndCorners
{
    public class Building_CellWall : Building
    {
        private Dictionary<string, Graphic> connectorGraphicsCache;

        public override Graphic Graphic
        {
            get
            {
                var baseGraphic = base.Graphic;
                if (Spawned)
                {
                    string connectorKey = DetermineConnectorKey();
                    if (connectorKey != null)
                    {
                        if (connectorGraphicsCache == null)
                        {
                            connectorGraphicsCache = new Dictionary<string, Graphic>();
                        }

                        if (!connectorGraphicsCache.TryGetValue(connectorKey, out Graphic cachedGraphic))
                        {
                            var copy = new GraphicData();
                            copy.CopyFrom(def.graphicData);
                            copy.texPath += $"_{connectorKey}";
                            cachedGraphic = copy.GraphicColoredFor(this);
                            connectorGraphicsCache[connectorKey] = cachedGraphic;
                        }
                        return connectorGraphicsCache[connectorKey];
                    }
                }
                return baseGraphic;
            }
        }

        private string DetermineConnectorKey()
        {
            var hasWallNorth = new Lazy<bool>(() => HasWall(Rot4.North));
            var hasWallSouth = new Lazy<bool>(() => HasWall(Rot4.South));
            var hasWallEast = new Lazy<bool>(() => HasWall(Rot4.East));
            var hasWallWest = new Lazy<bool>(() => HasWall(Rot4.West));

            if ((hasWallNorth.Value && hasWallSouth.Value) || (hasWallWest.Value && hasWallEast.Value))
            {
                return "BothSidesConnector";
            }
            if (hasWallNorth.Value || hasWallEast.Value)
            {
                return "TopRightConnector";
            }
            if (hasWallSouth.Value || hasWallWest.Value)
            {
                return "BottomLeftConnector";
            }
            return null;
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
