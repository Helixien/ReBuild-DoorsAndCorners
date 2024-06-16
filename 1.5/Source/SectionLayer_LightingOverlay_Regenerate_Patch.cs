using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(SectionLayer_LightingOverlay), nameof(SectionLayer_LightingOverlay.Regenerate))]
    public static class SectionLayer_LightingOverlay_Regenerate_Patch
    {
        public static Map curMap;
        public static MapComponent_Rebuild curComp;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var roofAt = AccessTools.Method(typeof(RoofGrid), "RoofAt", [typeof(int)]);
            var roofed = AccessTools.Method(typeof(RoofGrid), "Roofed", [typeof(int)]);
            foreach (var codeInstruction in codeInstructions)
            {
                yield return codeInstruction;
                if (codeInstruction.Calls(roofAt))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 28);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(SectionLayer_LightingOverlay_Regenerate_Patch), "TryInterceptRoof"));
                }
                else if (codeInstruction.Calls(roofed))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 34);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(SectionLayer_LightingOverlay_Regenerate_Patch), "TryInterceptRoofed"));
                }
            }
        }

        public static RoofDef TryInterceptRoof(RoofDef roof, SectionLayer_LightingOverlay __instance, int index)
        {
            if (curMap != __instance.Map)
            {
                curMap = __instance.Map;
                curComp = __instance.Map.GetComponent<MapComponent_Rebuild>();
            }
            if (curMap != null)
            {
                var cell = curMap.cellIndices.IndexToCell(index);
                if (curComp.cellsNearbyGlassWalls.Contains(cell))
                {
                    return null;
                }
            }
            return roof;
        }

        public static bool TryInterceptRoofed(bool roofed, SectionLayer_LightingOverlay __instance, int index)
        {
            if (curMap != __instance.Map)
            {
                curMap = __instance.Map;
                curComp = __instance.Map.GetComponent<MapComponent_Rebuild>();
            }
            if (curMap != null)
            {
                var cell = curMap.cellIndices.IndexToCell(index);
                if (curComp.cellsNearbyGlassWalls.Contains(cell))
                {
                    return false;
                }
            }
            return roofed;
        }
    }
}
