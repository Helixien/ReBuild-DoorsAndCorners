using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static Verse.TerrainDef;

namespace ReBuildDoorsAndCorners
{
    public class MapComponent_Rebuild : MapComponent
    {
        public HashSet<CompGlassWall> glassWalls = new HashSet<CompGlassWall>();
        public HashSet<IntVec3> cellsNearbyGlassWalls = new HashSet<IntVec3>();
        public MapComponent_Rebuild(Map map) : base(map)
        {
        }


        public bool regenerate;
        public HashSet<IntVec3> cellsToRegenerate = new();
        public override void MapComponentUpdate()
        {
            base.MapComponentUpdate();
            if (regenerate)
            {
                regenerate = false;
                foreach (var cell in cellsToRegenerate)
                {
                    map.mapDrawer.MapMeshDirty(cell, MapMeshFlagDefOf.Roofs);
                    map.glowGrid.DirtyCache(cell);
                }
            }
        }

        public void BuildCellsCache()
        {
            var oldCells = cellsNearbyGlassWalls.ToHashSet();
            cellsNearbyGlassWalls = new HashSet<IntVec3>();
            foreach (var wall in glassWalls)
            {
                if (wall.Props.naturalLightRadius.HasValue && wall.parent.OccupiedRect().ExpandedBy(1).EdgeCells.Any(x => x.Roofed(map) is false))
                {
                    foreach (var cell in GenRadial.RadialCellsAround(wall.parent.Position, wall.Props.naturalLightRadius.Value, true))
                    {
                        if (cell.InBounds(map) && cell.Roofed(map))
                        {
                            cellsNearbyGlassWalls.Add(cell);
                        }
                    }
                }
            }

            if (cellsNearbyGlassWalls.SetEquals(oldCells) is false)
            {
                cellsToRegenerate = new HashSet<IntVec3>();
                cellsToRegenerate.AddRange(cellsNearbyGlassWalls);
                cellsToRegenerate.AddRange(oldCells);
                regenerate = true;
            }
        }
    }
}
