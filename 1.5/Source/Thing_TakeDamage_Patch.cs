using HarmonyLib;
using UnityEngine;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(Thing), "TakeDamage")]
    public static class Thing_TakeDamage_Patch
    {
        public static void Prefix(Thing __instance, ref DamageInfo dinfo)
        {
            if (__instance.Spawned && __instance.def != RB_DefOf.RB_OverwallArmour)
            {
                var armor = __instance.Position.GetFirstThing(__instance.Map, RB_DefOf.RB_OverwallArmour);
                if (armor != null)
                {
                    var damageToApply = Mathf.Min(armor.HitPoints, dinfo.Amount);
                    var copy = new DamageInfo(dinfo);
                    copy.SetAmount(damageToApply);
                    armor.TakeDamage(copy);
                    var newDamage = dinfo.Amount - damageToApply;
                    Log.Message("newDamage: " + newDamage);
                    if (newDamage > 0)
                    {
                        dinfo.SetAmount(newDamage);
                    }
                    else
                    {
                        dinfo.SetAmount(0);
                    }
                }
            }
            Log.Message("dinfo: " + dinfo + " - " + __instance + " - " + __instance.HitPoints); ;
        }

        public static void Postfix(Thing __instance, DamageInfo dinfo)
        {
            Log.Message("2 dinfo: " + dinfo + " - " + __instance + " - " + __instance.HitPoints);
        }
    }
}
