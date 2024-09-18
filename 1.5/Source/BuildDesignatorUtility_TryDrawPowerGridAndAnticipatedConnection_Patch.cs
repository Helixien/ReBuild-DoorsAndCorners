using HarmonyLib;
using RimWorld;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(BuildDesignatorUtility), "TryDrawPowerGridAndAnticipatedConnection")]
    public static class BuildDesignatorUtility_TryDrawPowerGridAndAnticipatedConnection_Patch
    {
        public static bool Prefix(BuildableDef def)
        {
            if (def.ShouldPreventDrawingWires())
            {
                return false;
            }
            return true;
        }

        public static bool ShouldPreventDrawingWires(this BuildableDef def)
        {
            if (def != null)
            {
                return def == RB_DefOf.RB_FloorLamp || def == RB_DefOf.RB_ACUnitFloor;
            }
            return false;
        }
    }
}
