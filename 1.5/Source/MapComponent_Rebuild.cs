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
        public Dictionary<IntVec3, TerrainEdgeType> customTerrainEdges = new Dictionary<IntVec3, TerrainEdgeType>();
        public MapComponent_Rebuild(Map map) : base(map)
        {
        }

        public void BuildCellsCache()
        {
            cellsNearbyGlassWalls = new HashSet<IntVec3>();
            foreach (var wall in glassWalls)
            {
                if (wall.parent.OccupiedRect().ExpandedBy(1).EdgeCells.Any(x => x.Roofed(map) is false))
                {
                    foreach (var cell in GenRadial.RadialCellsAround(wall.parent.Position, 3, true))
                    {
                        if (cell.InBounds(map) && cell.Roofed(map))
                        {
                            cellsNearbyGlassWalls.Add(cell);
                        }
                    }
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref customTerrainEdges, "customTerrainEdges", LookMode.Value, LookMode.Value);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                customTerrainEdges ??= new Dictionary<IntVec3, TerrainEdgeType>();
            }
        }
    }
}
