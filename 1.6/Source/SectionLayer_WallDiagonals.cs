using System;
using System.Collections.Generic;
using System.Linq;
using ModSettingsFramework;
using RimWorld;
using UnityEngine;
using Verse;

namespace ReBuildDoorsAndCorners
{
    public class ReBuild_DiagonalWallToggles : PatchOperationWorker
    {
        public Dictionary<string, bool> diagonalWallToggles = new Dictionary<string, bool>();
        public override void ExposeData()
        {
            Scribe_Collections.Look(ref diagonalWallToggles, "diagonalWallToggles", LookMode.Value, LookMode.Value);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                diagonalWallToggles ??= new Dictionary<string, bool>();
            }
        }

        public override void CopyFrom(PatchOperationWorker savedWorker)
        {
            var copy = savedWorker as ReBuild_DiagonalWallToggles;
            diagonalWallToggles = copy.diagonalWallToggles;
        }
        public override void ApplySettings()
        {
            base.ApplySettings();
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs.ToList())
            {
                if (thingDef.HasModExtension<WallDiagonalExtension>() || ThingDefOf.GravshipHull == thingDef)
                {
                    if (!diagonalWallToggles.TryGetValue(thingDef.defName, out var state))
                    {
                        diagonalWallToggles[thingDef.defName] = state = true;
                    }
                }
            }
        }
        public override void DoSettings(ModSettingsContainer container, Listing_Standard list)
        {
            foreach (var diagonalWallState in diagonalWallToggles.ToList())
            {
                var diagonalWall = DefDatabase<ThingDef>.GetNamedSilentFail(diagonalWallState.Key);
                if (diagonalWall != null)
                {
                    var value = diagonalWallState.Value;
                    CheckboxLabeled(list, "RE.EnableDiagonal".Translate(diagonalWall.label), ref value);
                    diagonalWallToggles[diagonalWallState.Key] = value;
                    list.Gap(5);
                }
            }
        }

        public override int SettingsHeight()
        {
            var scrollHeight = 0;
            foreach (var diagonalWallState in diagonalWallToggles.ToList())
            {
                var diagonalWall = DefDatabase<ThingDef>.GetNamedSilentFail(diagonalWallState.Key);
                if (diagonalWall != null && diagonalWall.HasModExtension<WallDiagonalExtension>() || ThingDefOf.GravshipHull == diagonalWall)
                {
                    scrollHeight += 29;
                }
            }
            return scrollHeight;
        }

        public override void Reset()
        {
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs.ToList())
            {
                if (thingDef.HasModExtension<WallDiagonalExtension>() || ThingDefOf.GravshipHull == thingDef)
                {
                    diagonalWallToggles[thingDef.defName] = true;
                }
            }
        }
    }
    
    [StaticConstructorOnStartup]
    public class SectionLayer_WallDiagonals : SectionLayer
    {
        private static ReBuild_DiagonalWallToggles _diagonalWallsPatchWorker;
        public static ReBuild_DiagonalWallToggles DiagonalWallPatchWorker => _diagonalWallsPatchWorker ??= ReBuildDoorsAndCornersMod.modInstance
            .Patches.OfType<ReBuild_DiagonalWallToggles>().FirstOrDefault();
        public enum CornerType
        {
            None,
            Corner_NW, Corner_NE, Corner_SW, Corner_SE,
            Diagonal_NW, Diagonal_NE, Diagonal_SW, Diagonal_SE
        }
        private static Dictionary<ThingDef, ThingCornerData> dataCache = new Dictionary<ThingDef, ThingCornerData>();
        private static readonly float DefaultCornerAltitude = AltitudeLayer.BuildingOnTop.AltitudeFor();
        private static readonly Vector2[] UVs = new Vector2[4]
        {
            new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f)
        };
        private static readonly IntVec3[] Directions = new IntVec3[8]
        {
            IntVec3.North, IntVec3.East, IntVec3.South, IntVec3.West,
            IntVec3.North + IntVec3.West, IntVec3.North + IntVec3.East,
            IntVec3.South + IntVec3.East, IntVec3.South + IntVec3.West
        };
        private static bool[] tmpChecks = new bool[Directions.Length];

        public SectionLayer_WallDiagonals(Section section) : base(section)
        {
            relevantChangeTypes = (ulong)MapMeshFlagDefOf.Buildings | (ulong)MapMeshFlagDefOf.Terrain | (ulong)MapMeshFlagDefOf.Things | (ulong)MapMeshFlagDefOf.Roofs;
        }

        public override void Regenerate()
        {
            ClearSubMeshes(MeshParts.All);
            Map map = base.Map;

            foreach (IntVec3 cell in section.CellRect)
            {
                if (ShouldDrawCornerPiece(cell, map, out CornerType cornerType, out Thing sourceThing))
                {
                    ThingCornerData data = GetDataFor(sourceThing.def);
                    if (data == null) continue;

                    Material material = data.GetMaterial(cornerType);
                    IntVec3 offset = GetOffset(cornerType);
                    bool addIndoorMask = IsCornerIndoorMasked(cell, cornerType, map);

                    AddQuad(material, cell + offset, data.CornerScale, data.Altitude, sourceThing.DrawColor, addIndoorMask);
                }
            }
            FinalizeMesh(MeshParts.All);
        }

        public static bool ShouldDrawCornerPiece(IntVec3 pos, Map map, out CornerType cornerType, out Thing sourceThing)
        {
            cornerType = CornerType.None;
            sourceThing = null;

            if (pos.GetEdifice(map) != null)
            {
                return false;
            }

            Thing definingThing = null;
            for (int i = 0; i < 4; i++)
            {
                Thing thing = (pos + Directions[i]).GetEdificeSafe(map);
                if (thing != null && thing.def.HasModExtension<WallDiagonalExtension>())
                {
                    if (!DiagonalWallPatchWorker.diagonalWallToggles.TryGetValue(thing.def.defName, out var state) || !state)
                    {
                        continue;
                    }
                    definingThing = thing;
                    break;
                }
            }

            if (definingThing == null)
            {
                return false;
            }

            sourceThing = definingThing;

            for (int i = 0; i < Directions.Length; i++)
            {
                Thing thing = (pos + Directions[i]).GetEdificeSafe(map);
                tmpChecks[i] = thing != null && thing.def == definingThing.def;
            }

            if (tmpChecks[0] && tmpChecks[3] && !tmpChecks[2] && !tmpChecks[1])
                cornerType = tmpChecks[4] ? CornerType.Corner_NW : CornerType.Diagonal_NW;
            else if (tmpChecks[0] && tmpChecks[1] && !tmpChecks[2] && !tmpChecks[3])
                cornerType = tmpChecks[5] ? CornerType.Corner_NE : CornerType.Diagonal_NE;
            else if (tmpChecks[2] && tmpChecks[1] && !tmpChecks[0] && !tmpChecks[3])
                cornerType = tmpChecks[6] ? CornerType.Corner_SE : CornerType.Diagonal_SE;
            else if (tmpChecks[2] && tmpChecks[3] && !tmpChecks[0] && !tmpChecks[1])
                cornerType = tmpChecks[7] ? CornerType.Corner_SW : CornerType.Diagonal_SW;

            return cornerType != CornerType.None;
        }

        private static bool IsCornerIndoorMasked(IntVec3 c, CornerType cornerType, Map map)
        {
            switch (cornerType)
            {
                case CornerType.Corner_NE:
                case CornerType.Diagonal_NE:
                    return c.Roofed(map) || (c + IntVec3.North).Roofed(map) || (c + IntVec3.East).Roofed(map);
                case CornerType.Corner_NW:
                case CornerType.Diagonal_NW:
                    return c.Roofed(map) || (c + IntVec3.North).Roofed(map) || (c + IntVec3.West).Roofed(map);
                case CornerType.Corner_SE:
                case CornerType.Diagonal_SE:
                    return c.Roofed(map) || (c + IntVec3.South).Roofed(map) || (c + IntVec3.East).Roofed(map);
                case CornerType.Corner_SW:
                case CornerType.Diagonal_SW:
                    return c.Roofed(map) || (c + IntVec3.South).Roofed(map) || (c + IntVec3.West).Roofed(map);
                default:
                    return false;
            }
        }

        private void AddQuad(Material mat, IntVec3 c, float scale, float altitude, Color color, bool addIndoorMask)
        {
            LayerSubMesh subMesh = GetSubMesh(mat);
            AddQuad(subMesh, c.ToVector3(), scale, altitude, color);

            if (addIndoorMask)
            {
                Texture2D srcTex = subMesh.material.mainTexture as Texture2D;
                Color matColor = subMesh.material.color;
                Material material = MaterialPool.MatFrom(srcTex, ShaderDatabase.IndoorMaskMasked, matColor);
                AddQuad(GetSubMesh(material), c.ToVector3(), scale, altitude, color);
            }
        }

        private static void AddQuad(LayerSubMesh sm, Vector3 c, float scale, float altitude, Color color)
        {
            int count = sm.verts.Count;
            sm.tris.Add(count);
            sm.tris.Add(count + 1);
            sm.tris.Add(count + 2);
            sm.tris.Add(count);
            sm.tris.Add(count + 2);
            sm.tris.Add(count + 3);

            for (int i = 0; i < 4; i++)
            {
                sm.verts.Add(new Vector3(c.x + UVs[i].x * scale, altitude, c.z + UVs[i].y * scale));
                sm.uvs.Add(UVs[i % 4]);
                sm.colors.Add(color);
            }
        }

        private static IntVec3 GetOffset(CornerType cornerType)
        {
            switch (cornerType)
            {
                case CornerType.Corner_NW:
                case CornerType.Diagonal_NW:
                    return new IntVec3(-1, 0, 0);
                case CornerType.Corner_SE:
                case CornerType.Diagonal_SE:
                    return new IntVec3(0, 0, -1);
                case CornerType.Corner_SW:
                case CornerType.Diagonal_SW:
                    return new IntVec3(-1, 0, -1);
                default:
                    return IntVec3.Zero;
            }
        }

        private static ThingCornerData GetDataFor(ThingDef def)
        {
            if (dataCache.TryGetValue(def, out ThingCornerData data))
            {
                return data;
            }
            var extension = def.GetModExtension<WallDiagonalExtension>();
            if (extension == null) return null;

            var newData = new ThingCornerData(extension);
            dataCache.Add(def, newData);
            return newData;
        }

        private class ThingCornerData
        {
            private readonly Material mat_Corner_NW, mat_Corner_NE, mat_Corner_SW, mat_Corner_SE;
            private readonly Material mat_Diagonal_NW, mat_Diagonal_NE, mat_Diagonal_SW, mat_Diagonal_SE;
            public readonly float CornerScale;
            public readonly float Altitude;

            public ThingCornerData(WallDiagonalExtension ext)
            {
                Shader shader = ext.shaderType?.Shader ?? ShaderDatabase.CutoutOverlay;
                mat_Corner_NW = MaterialPool.MatFrom(ext.texPath_Corner_NW, shader);
                mat_Corner_NE = MaterialPool.MatFrom(ext.texPath_Corner_NE, shader);
                mat_Corner_SW = MaterialPool.MatFrom(ext.texPath_Corner_SW, shader);
                mat_Corner_SE = MaterialPool.MatFrom(ext.texPath_Corner_SE, shader);
                mat_Diagonal_NW = MaterialPool.MatFrom(ext.texPath_Diagonal_NW, shader);
                mat_Diagonal_NE = MaterialPool.MatFrom(ext.texPath_Diagonal_NE, shader);
                mat_Diagonal_SW = MaterialPool.MatFrom(ext.texPath_Diagonal_SW, shader);
                mat_Diagonal_SE = MaterialPool.MatFrom(ext.texPath_Diagonal_SE, shader);

                CornerScale = ext.cornerScale;
                Altitude = ext.altitude >= 0 ? ext.altitude : DefaultCornerAltitude;
            }

            public Material GetMaterial(CornerType cornerType)
            {
                switch (cornerType)
                {
                    case CornerType.Corner_NW: return mat_Corner_NW;
                    case CornerType.Corner_NE: return mat_Corner_NE;
                    case CornerType.Corner_SW: return mat_Corner_SW;
                    case CornerType.Corner_SE: return mat_Corner_SE;
                    case CornerType.Diagonal_NW: return mat_Diagonal_NW;
                    case CornerType.Diagonal_NE: return mat_Diagonal_NE;
                    case CornerType.Diagonal_SW: return mat_Diagonal_SW;
                    case CornerType.Diagonal_SE: return mat_Diagonal_SE;
                    default: throw new ArgumentOutOfRangeException(nameof(cornerType));
                }
            }
        }
    }
}
