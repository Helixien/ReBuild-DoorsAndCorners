using HarmonyLib;
using ModSettingsFramework;
using RimWorld;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(Building_Door), "PawnCanOpen")]
    public static class Building_Door_PawnCanOpen_Patch
    {
        public static void Postfix(ref bool __result, Pawn p, Building_Door __instance)
        {
            if (__result && p.IsEntity && (__instance.def == ThingDefOf.SecurityDoor || __instance.def == RB_DefOf.RB_LargeSecurityDoor) 
                && ReBuildDoorsAndCornersMod.modInstance.GetModOptionState(ReBuildDoorsAndCornersMod.RB_EntitiesCannotOpenSecurityDoors))
            {
                __result = false;
            }
        }
    }
}
