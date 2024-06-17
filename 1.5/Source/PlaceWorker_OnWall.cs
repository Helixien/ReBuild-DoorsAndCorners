﻿using VanillaFurnitureExpanded;
using Verse;

namespace ReBuildDoorsAndCorners
{
    public class PlaceWorker_OnWall : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            if (loc.InBounds(map))
            {
                var edifice = loc.GetEdifice(map);
                if (edifice != null && edifice.def.IsWall())
                {
                    return true;
                }
            }
            return "RE.WallArmorMustBePlacedOnWall".Translate();
        }
    }
}
