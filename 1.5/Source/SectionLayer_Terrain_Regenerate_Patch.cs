using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using static Verse.TerrainDef;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(SectionLayer_Terrain), nameof(SectionLayer_Terrain.Regenerate))]
    public static class SectionLayer_Terrain_Regenerate_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var edgeType = AccessTools.Field(typeof(TerrainDef), nameof(TerrainDef.edgeType));
            var getMaterial = AccessTools.Method(typeof(SectionLayer_Terrain), "GetMaterialFor");
            var codes = instructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                yield return code;
                if (code.LoadsField(edgeType))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SectionLayer), "get_Map"));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SectionLayer_Terrain_Regenerate_Patch), nameof(TryOverrideTerrainEdge)));
                }
                else if (code.Calls(getMaterial))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SectionLayer), "get_Map"));
                    yield return codes[i - 1];
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SectionLayer_Terrain_Regenerate_Patch), nameof(TryOverrideMaterial)));
                }
                else if (code.opcode == OpCodes.Stloc_2)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 10);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SectionLayer_Terrain_Regenerate_Patch), nameof(MapTerrain)));
                }
                else if (code.opcode == OpCodes.Stloc_S && code.operand is LocalBuilder lb && lb.LocalIndex == 8)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 13);
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SectionLayer_Terrain_Regenerate_Patch), nameof(MapTerrain)));
                }
            }
        }

        public static void MapTerrain(CellTerrain cellTerrain, IntVec3 cell)
        {
            mappedTerrains[cellTerrain] = cell;
        }

        public static Map curMap;
        public static MapComponent_Rebuild curComp;
        public static TerrainEdgeType TryOverrideTerrainEdge(TerrainEdgeType type, IntVec3 cell, Map map)
        {
            if (curMap != map)
            {
                curMap = map;
                curComp = map.GetComponent<MapComponent_Rebuild>();
            }
            if (TryGetCustomEdge(cell, map, out var edge))
            {
                return edge;
            }
            return type;
        }

        private static bool TryGetCustomEdge(IntVec3 cell, Map map, out TerrainEdgeType edge)
        {
            if (curComp.customTerrainEdges.TryGetValue(cell, out edge))
            {
                if (cell.GetTerrain(map).natural is false)
                {
                    return true;
                }
                else if (GenAdj.CellsAdjacent8Way(cell, Rot4.South, IntVec2.One).Any(x => x.GetTerrain(map).natural is false && curComp.customTerrainEdges.ContainsKey(x)))
                {
                    return true;
                }
            }
            return false;
        }

        public static Dictionary<CellTerrain, IntVec3> mappedTerrains = new Dictionary<CellTerrain, IntVec3>();

        public static Material TryOverrideMaterial(Material material, Map map, CellTerrain cellTerrain)
        {
            var cell = mappedTerrains[cellTerrain];
            if (curMap != map)
            {
                curMap = map;
                curComp = map.GetComponent<MapComponent_Rebuild>();
            }
            if (TryGetCustomEdge(cell, map, out var edge))
            {
                if (edge == TerrainEdgeType.Hard) return material;
                bool polluted = cellTerrain.polluted && cellTerrain.snowCoverage < 0.4f 
                    && cellTerrain.def.graphicPolluted != BaseContent.BadGraphic;
                var def = cellTerrain.def;
                var color = cellTerrain.color;
                Graphic graphic = (polluted ? def.graphicPolluted : def.graphic);
                if (color != null)
                {
                    return graphic.GetColoredVersion(Shader(edge), color.color, Color.white).MatSingle;
                }
                else
                {
                    if (polluted && def.graphicPolluted != null)
                    {
                        return def.graphicPolluted.GetCopy(Vector2.one, ShaderPolluted(edge)).MatSingle;
                    }
                    return def.graphic.GetCopy(Vector2.one, Shader(edge)).MatSingle;
                }
            }
            return material;
        }

        public static Shader Shader(TerrainEdgeType edgeType)
        {
            Shader result = null;
            switch (edgeType)
            {
                case TerrainEdgeType.Hard:
                    result = ShaderDatabase.TerrainHard;
                    break;
                case TerrainEdgeType.Fade:
                    result = ShaderDatabase.TerrainFade;
                    break;
                case TerrainEdgeType.FadeRough:
                    result = ShaderDatabase.TerrainFadeRough;
                    break;
            }
            return result;
        }

        private static Shader ShaderPolluted(TerrainEdgeType edgeType)
        {
            Shader result = null;
            switch (edgeType)
            {
                case TerrainEdgeType.Hard:
                    result = ShaderDatabase.TerrainHardPolluted;
                    break;
                case TerrainEdgeType.Fade:
                    result = ShaderDatabase.TerrainFadePolluted;
                    break;
                case TerrainEdgeType.FadeRough:
                    result = ShaderDatabase.TerrainFadeRoughPolluted;
                    break;
            }
            return result;
        }
    }
}
