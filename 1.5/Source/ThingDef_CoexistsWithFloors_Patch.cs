using HarmonyLib;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(ThingDef), "CoexistsWithFloors", MethodType.Getter)]
    public static class ThingDef_CoexistsWithFloors_Patch
    {
        public static void Postfix(ref bool __result, ThingDef __instance)
        {
            if (__result is false && __instance.HasComp<CompGlassWall>())
            {
                __result = true;
            }
        }
    }
}
