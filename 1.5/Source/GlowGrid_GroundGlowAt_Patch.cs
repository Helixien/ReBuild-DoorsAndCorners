using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(GlowGrid), "GroundGlowAt")]
    public static class GlowGrid_GroundGlowAt_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
        {
            var roofed = AccessTools.Method(typeof(RoofGrid), "Roofed", [typeof(IntVec3)]);
            var hasNoNaturalLight = AccessTools.Method(typeof(GlowGrid_GroundGlowAt_Patch), "HasNoNaturalLight");
            foreach (var code in codes)
            {
                yield return code;
                if (code.Calls(roofed))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, hasNoNaturalLight);
                }
            }
        }

        public static Map curMap;
        public static MapComponent_Rebuild curComp;
        public static bool HasNoNaturalLight(bool roofed, IntVec3 c, GlowGrid glowGrid)
        {
            if (curMap != glowGrid.map)
            {
                curMap = glowGrid.map;
                curComp = glowGrid.map.GetComponent<MapComponent_Rebuild>();
            }
            if (curComp.cellsNearbyGlassWalls.Contains(c))
            {
                return false;
            }
            return roofed;
        }
    }
}
