using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(LordJob_Ritual), MethodType.Constructor, new Type[] { typeof(TargetInfo),
        typeof(Precept_Ritual), typeof(RitualObligation), typeof(List<RitualStage>),
        typeof(RitualRoleAssignments), typeof(Pawn), typeof(IntVec3?)})]
    public static class LordJob_Ritual_Constructor_Patch
    {
        public static void Postfix(LordJob_Ritual __instance)
        {
            if (__instance.selectedTarget.Thing?.def == RB_DefOf.RB_OverwallFireplace)
            {
                __instance.spot += __instance.selectedTarget.Thing.Rotation.FacingCell;
            }
        }
    }
}
