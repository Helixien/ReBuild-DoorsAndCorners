using HarmonyLib;
using RimWorld;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(SectionLayer_GravshipHull), "Regenerate")]
    public static class SectionLayer_GravshipHull_Regenerate_Patch
    {
        public static bool Prefix()
        {
            if (SectionLayer_WallDiagonals.DiagonalWallPatchWorker.diagonalWallToggles.TryGetValue(ThingDefOf.GravshipHull.defName, out var toggle) && toggle is false)
            {
                return false;
            }
            return true;
        }
    }
}
