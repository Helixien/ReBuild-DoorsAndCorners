using Verse;
using HarmonyLib;
using RimWorld;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(Building_Door), "DrawAt")]
    public static class Building_Door_DrawAt_Patch
    {
        public static void Postfix(Building_Door __instance)
        {
            CompAdditionalGraphic comp = __instance.TryGetComp<CompAdditionalGraphic>();
            if (comp != null)
            {
                comp.PostDraw();
            }
        }
    }
}