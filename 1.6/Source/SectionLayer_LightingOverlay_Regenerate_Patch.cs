using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;
using Verse.AI;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(SectionLayer_LightingOverlay), nameof(SectionLayer_LightingOverlay.GenerateLightingOverlay))]
    public static class SectionLayer_LightingOverlay_GenerateLightingOverlay_Patch
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
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 26);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(SectionLayer_LightingOverlay_GenerateLightingOverlay_Patch), "TryInterceptRoof"));
                }
                else if (codeInstruction.Calls(roofed))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 32);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(SectionLayer_LightingOverlay_GenerateLightingOverlay_Patch), "TryInterceptRoofed"));
                }
            }
        }

        public static RoofDef TryInterceptRoof(RoofDef roof, Map map, int index)
        {
            if (curMap != map)
            {
                curMap = map;
                curComp = map.GetComponent<MapComponent_Rebuild>();
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

        public static bool TryInterceptRoofed(bool roofed, Map map, int index)
        {
            if (curMap != map)
            {
                curMap = map;
                curComp = map.GetComponent<MapComponent_Rebuild>();
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
