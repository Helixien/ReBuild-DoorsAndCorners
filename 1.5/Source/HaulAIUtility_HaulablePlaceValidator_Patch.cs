using HarmonyLib;
using ModSettingsFramework;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(HaulAIUtility), "HaulablePlaceValidator")]
    public static class HaulAIUtility_HaulablePlaceValidator_Patch
    {
        public static bool Prefix(Pawn worker, IntVec3 c)
        {
            if (ReBuildDoorsAndCornersMod.modInstance.GetModOptionState(ReBuildDoorsAndCornersMod.RB_ColonistsDontDropItemsAtDoors))
            {
                if (c.GetFirstThing<Building_Door>(worker.Map) != null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
