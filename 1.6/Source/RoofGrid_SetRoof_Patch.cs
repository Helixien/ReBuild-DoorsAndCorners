using HarmonyLib;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(RoofGrid), "SetRoof")]
    public static class RoofGrid_SetRoof_Patch
    {
        public static void Postfix(RoofGrid __instance, IntVec3 c, RoofDef def, Map ___map)
        {
            var comp = ___map.GetComponent<MapComponent_Rebuild>();
            comp.BuildCellsCache();
        }
    }
}
