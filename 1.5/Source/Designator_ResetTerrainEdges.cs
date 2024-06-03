using RimWorld;
using UnityEngine;
using Verse;

namespace ReBuildDoorsAndCorners
{
    public class Designator_ResetTerrainEdges : Designator_Cells
    {
        public override int DraggableDimensions => 2;

        public override bool DragDrawMeasurements => true;

        public Designator_ResetTerrainEdges()
        {
            defaultLabel = "RE.ResetTerrainEdgesLabel".Translate();
            defaultDesc = "RE.ResetTerrainEdgesDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/ResetTerrainEdges");
            hotKey = KeyBindingDefOf.Misc9;
            soundDragSustain = SoundDefOf.Designate_DragAreaAdd;
            soundDragChanged = SoundDefOf.Designate_DragZone_Changed;
            soundSucceeded = SoundDefOf.Designate_ZoneAdd_Roof;
            useMouseIcon = true;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!c.InBounds(base.Map))
            {
                return false;
            }
            return true;
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            var comp = base.Map.GetComponent<MapComponent_Rebuild>();
            foreach (var cell in GenAdj.CellsAdjacent8Way(c, Rot4.South, IntVec2.One))
            {
                comp.customTerrainEdges.Remove(cell);
                base.Map.mapDrawer.SectionAt(cell).dirtyFlags = MapMeshFlagDefOf.Terrain;
            }

        }
    }
}
