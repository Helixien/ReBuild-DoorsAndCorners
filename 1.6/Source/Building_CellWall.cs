using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HotSwappableAttribute : Attribute
    {
    }

    [HotSwappable]
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

        public override void Notify_ColorChanged()
        {
            base.Notify_ColorChanged();
            connectorGraphicsCache = null;
        }

        private string DetermineConnectorKey()
        {
            var diffWallNorth = new Lazy<bool>(() => HasDifferentWall(Rot4.North));
            var diffWallSouth = new Lazy<bool>(() => HasDifferentWall(Rot4.South));
            var diffWallEast = new Lazy<bool>(() => HasDifferentWall(Rot4.East));
            var diffWallWest = new Lazy<bool>(() => HasDifferentWall(Rot4.West));

            var doorNorth = new Lazy<bool>(() => HasDoor(Rot4.North));
            var doorSouth = new Lazy<bool>(() => HasDoor(Rot4.South));
            var doorEast = new Lazy<bool>(() => HasDoor(Rot4.East));
            var doorWest = new Lazy<bool>(() => HasDoor(Rot4.West));

            var sameWallNorth = new Lazy<bool>(() => HasSameWall(Rot4.North));
            var sameWallSouth = new Lazy<bool>(() => HasSameWall(Rot4.South));
            var sameWallEast = new Lazy<bool>(() => HasSameWall(Rot4.East));
            var sameWallWest = new Lazy<bool>(() => HasSameWall(Rot4.West));

            if ((sameWallEast.Value && (doorWest.Value || doorNorth.Value || doorSouth.Value))
                || (sameWallWest.Value && (doorEast.Value || doorNorth.Value || doorSouth.Value))
                || (sameWallNorth.Value && (doorEast.Value || doorWest.Value || doorSouth.Value))
                || (sameWallSouth.Value && (doorEast.Value || doorWest.Value || doorNorth.Value)))
            {
                return null;
            }

            if ((diffWallNorth.Value && diffWallSouth.Value)
                || (diffWallWest.Value && diffWallEast.Value))
            {
                return "BothSidesConnector";
            }

            if (diffWallWest.Value && sameWallSouth.Value)
            {
                return "TopRightConnector";
            }
            if (diffWallEast.Value && sameWallNorth.Value)
            {
                return "BottomLeftConnector";
            }
            if (diffWallEast.Value)
            {
                return "TopRightConnector";
            }
            if (diffWallWest.Value)
            {
                return "BottomLeftConnector";
            }
            if (diffWallNorth.Value)
            {
                return "TopRightConnector";
            }
            if (diffWallSouth.Value)
            {
                return "BottomLeftConnector";
            }
            return null;
        }

        private bool HasDifferentWall(Rot4 rot)
        {
            var edifice = (rot.FacingCell + Position).GetEdifice(Map);
            if (edifice is null || edifice.def == this.def)
            {
                return false;
            }
            return edifice.def.Fillage == FillCategory.Full;
        }

        private bool HasDoor(Rot4 rot)
        {
            var door = (rot.FacingCell + Position).GetFirstThing<Building_Door>(Map);
            return door != null;
        }

        private bool HasSameWall(Rot4 rot)
        {
            var edifice = (rot.FacingCell + Position).GetEdifice(Map);
            if (edifice is null || edifice.def != this.def)
            {
                return false;
            }
            return edifice.def == this.def;
        }
    }
}
