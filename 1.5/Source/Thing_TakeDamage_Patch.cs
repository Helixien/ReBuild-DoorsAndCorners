using HarmonyLib;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(Thing), "TakeDamage")]
    public static class Thing_TakeDamage_Patch
    {
        public static void Prefix(Thing __instance, DamageInfo dinfo)
        {
            if (ModsConfig.AnomalyActive && __instance.def == RB_DefOf.RB_ContainmentWall 
                && dinfo.Instigator is Pawn pawn && pawn.IsEntity)
            {
                dinfo.SetAmount(dinfo.Amount / 2f);
            }
        }
    }
}
