using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using static Verse.TerrainDef;

namespace ReBuildDoorsAndCorners
{
    public class Designator_SetTerrainEdges : Designator_Cells
    {
        public TerrainEdgeType curEdgeType;
        public override int DraggableDimensions => 2;

        public override bool DragDrawMeasurements => true;

        public Designator_SetTerrainEdges()
        {
            defaultLabel = "RE.SetTerrainEdgesLabel".Translate();
            defaultDesc = "RE.SetTerrainEdgesDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/SetTerrainEdges");
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
            foreach (var cell in GenAdj.CellsAdjacentCardinal(c, Rot4.South, IntVec2.One).Append(c))
            {
                if (c == cell || cell.GetTerrain(Map).natural)
                {
                    comp.customTerrainEdges[cell] = curEdgeType;
                    base.Map.mapDrawer.SectionAt(cell).dirtyFlags = MapMeshFlagDefOf.Terrain;
                }
            }
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            var floatList = new List<FloatMenuOption>();
            foreach (var type in Enum.GetValues(typeof(TerrainEdgeType)).Cast<TerrainEdgeType>().Where(x => x != TerrainEdgeType.Water))
            {
                floatList.Add(new FloatMenuOption(("RE.TerrainEdgeType" + type.ToString()).Translate(), delegate
                {
                    this.curEdgeType = type;
                }));
            }
            Find.WindowStack.Add(new FloatMenu(floatList));
        }

        public override void SelectedUpdate()
        {
            base.SelectedUpdate();
            GenUI.RenderMouseoverBracket();
        }
    }
}
