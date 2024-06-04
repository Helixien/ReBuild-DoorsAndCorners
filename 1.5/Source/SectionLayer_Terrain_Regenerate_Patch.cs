using HarmonyLib;
using RimWorld;
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
        //public static int Order(IntVec3 cell, Map map)
        //{
        //    if (curMap != map)
        //    {
        //        curMap = map;
        //        curComp = map.GetComponent<MapComponent_Rebuild>();
        //    }
        //    if (TryGetCustomEdge(cell, map, out var edge))
        //    {
        //        if (cell.GetTerrain(map).natural)
        //        {
        //            return 1;
        //        }
        //        else
        //        {
        //            return 3;
        //        }
        //    }
        //    return 2;
        //}
        //public static bool Prefix(SectionLayer_Terrain __instance)
        //{
        //    __instance.ClearSubMeshes(MeshParts.All);
        //    TerrainGrid terrainGrid = __instance.Map.terrainGrid;
        //    CellRect cellRect = __instance.section.CellRect;
        //    CellTerrain[] array = new CellTerrain[8];
        //    HashSet<CellTerrain> hashSet = new HashSet<CellTerrain>();
        //    bool[] array2 = new bool[8];
        //    foreach (IntVec3 item in cellRect.OrderBy(x => Order(x, __instance.Map)))
        //    {
        //        hashSet.Clear();
        //        CellTerrain cellTerrain = new CellTerrain(terrainGrid.TerrainAt(item), item.IsPolluted(__instance.Map), __instance.Map.snowGrid.GetDepth(item), terrainGrid.ColorAt(item));
        //        MapTerrain(cellTerrain, item);
        //        LayerSubMesh subMesh = __instance.GetSubMesh(TryOverrideMaterial(__instance.GetMaterialFor(cellTerrain), __instance.Map, cellTerrain));
        //        if (subMesh != null && __instance.AllowRenderingFor(cellTerrain.def))
        //        {
        //            int count = subMesh.verts.Count;
        //            subMesh.verts.Add(new Vector3(item.x, 0f, item.z));
        //            subMesh.verts.Add(new Vector3(item.x, 0f, item.z + 1));
        //            subMesh.verts.Add(new Vector3(item.x + 1, 0f, item.z + 1));
        //            subMesh.verts.Add(new Vector3(item.x + 1, 0f, item.z));
        //            subMesh.colors.Add(SectionLayer_Terrain.ColorWhite);
        //            subMesh.colors.Add(SectionLayer_Terrain.ColorWhite);
        //            subMesh.colors.Add(SectionLayer_Terrain.ColorWhite);
        //            subMesh.colors.Add(SectionLayer_Terrain.ColorWhite);
        //            subMesh.tris.Add(count);
        //            subMesh.tris.Add(count + 1);
        //            subMesh.tris.Add(count + 2);
        //            subMesh.tris.Add(count);
        //            subMesh.tris.Add(count + 2);
        //            subMesh.tris.Add(count + 3);
        //        }
        //        for (int i = 0; i < 8; i++)
        //        {
        //            IntVec3 c = item + GenAdj.AdjacentCellsAroundBottom[i];
        //            if (!c.InBounds(__instance.Map))
        //            {
        //                array[i] = cellTerrain;
        //                continue;
        //            }
        //            CellTerrain cellTerrain2 = new CellTerrain(terrainGrid.TerrainAt(c), c.IsPolluted(__instance.Map), __instance.Map.snowGrid.GetDepth(c), terrainGrid.ColorAt(c));
        //            Thing edifice = c.GetEdifice(__instance.Map);
        //            MapTerrain(cellTerrain2, c);
        //            if (edifice != null && edifice.def.coversFloor)
        //            {
        //                cellTerrain2.def = TerrainDefOf.Underwall;
        //            }
        //            array[i] = cellTerrain2;
        //            if (!cellTerrain2.Equals(cellTerrain) && TryOverrideTerrainEdge(cellTerrain2.def.edgeType, c, __instance.Map) != 0 && cellTerrain2.def.renderPrecedence >= cellTerrain.def.renderPrecedence && !hashSet.Contains(cellTerrain2))
        //            {
        //                hashSet.Add(cellTerrain2);
        //            }
        //        }
        //        foreach (CellTerrain item2 in hashSet)
        //        {
        //            LayerSubMesh subMesh2 = __instance.GetSubMesh(TryOverrideMaterial(__instance.GetMaterialFor(item2), __instance.Map, item2));
        //            if (subMesh2 == null || !__instance.AllowRenderingFor(item2.def))
        //            {
        //                continue;
        //            }
        //            int count = subMesh2.verts.Count;
        //            subMesh2.verts.Add(new Vector3((float)item.x + 0.5f, 0f, item.z));
        //            subMesh2.verts.Add(new Vector3(item.x, 0f, item.z));
        //            subMesh2.verts.Add(new Vector3(item.x, 0f, (float)item.z + 0.5f));
        //            subMesh2.verts.Add(new Vector3(item.x, 0f, item.z + 1));
        //            subMesh2.verts.Add(new Vector3((float)item.x + 0.5f, 0f, item.z + 1));
        //            subMesh2.verts.Add(new Vector3(item.x + 1, 0f, item.z + 1));
        //            subMesh2.verts.Add(new Vector3(item.x + 1, 0f, (float)item.z + 0.5f));
        //            subMesh2.verts.Add(new Vector3(item.x + 1, 0f, item.z));
        //            subMesh2.verts.Add(new Vector3((float)item.x + 0.5f, 0f, (float)item.z + 0.5f));
        //            for (int j = 0; j < 8; j++)
        //            {
        //                array2[j] = false;
        //            }
        //            for (int k = 0; k < 8; k++)
        //            {
        //                if (k % 2 == 0)
        //                {
        //                    if (array[k].Equals(item2))
        //                    {
        //                        array2[(k - 1 + 8) % 8] = true;
        //                        array2[k] = true;
        //                        array2[(k + 1) % 8] = true;
        //                    }
        //                }
        //                else if (array[k].Equals(item2))
        //                {
        //                    array2[k] = true;
        //                }
        //            }
        //            for (int l = 0; l < 8; l++)
        //            {
        //                if (array2[l])
        //                {
        //                    subMesh2.colors.Add(SectionLayer_Terrain.ColorWhite);
        //                }
        //                else
        //                {
        //                    subMesh2.colors.Add(SectionLayer_Terrain.ColorClear);
        //                }
        //            }
        //            subMesh2.colors.Add(SectionLayer_Terrain.ColorClear);
        //            for (int m = 0; m < 8; m++)
        //            {
        //                subMesh2.tris.Add(count + m);
        //                subMesh2.tris.Add(count + (m + 1) % 8);
        //                subMesh2.tris.Add(count + 8);
        //            }
        //        }
        //    }
        //    __instance.FinalizeMesh(MeshParts.All);
        //    return false;
        //}

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
                else if (GenAdj.CellsAdjacent8Way(cell, Rot4.South, IntVec2.One)
                    .Any(x => x.GetTerrain(map).natural is false && curComp.customTerrainEdges.ContainsKey(x)))
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
