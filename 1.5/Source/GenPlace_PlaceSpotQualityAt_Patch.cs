using HarmonyLib;
using ModSettingsFramework;
using RimWorld;
using Verse;
using static Verse.GenPlace;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(GenPlace), "PlaceSpotQualityAt")]
    public static class GenPlace_PlaceSpotQualityAt_Patch
    {
        public static bool Prefix(IntVec3 c, Map map, ref PlaceSpotQuality __result)
        {
            if (ReBuildDoorsAndCornersMod.modInstance.GetModOptionState(ReBuildDoorsAndCornersMod.RB_ColonistsDontDropItemsAtDoors))
            {
                if (c.GetFirstThing<Building_Door>(map) != null)
                {
                    __result = PlaceSpotQuality.Unusable;
                    return false;
                }
            }
            return true;
        }
    }
}
