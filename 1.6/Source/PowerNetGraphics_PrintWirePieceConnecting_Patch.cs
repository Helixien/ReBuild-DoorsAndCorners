using HarmonyLib;
using RimWorld;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(PowerNetGraphics), "PrintWirePieceConnecting")]
    public static class PowerNetGraphics_PrintWirePieceConnecting_Patch
    {
        public static bool Prefix(Thing A, Thing B)
        {
            if (A.def.ShouldPreventDrawingWires() || B.def.ShouldPreventDrawingWires())
            {
                return false;
            }
            return true;
        }
    }
}
