using HarmonyLib;
using System.Diagnostics;
using Verse;
using Verse.Sound;

namespace ReBuildDoorsAndCorners
{
    public class CompProperties_GlassWall : CompProperties
    {
        public float? naturalLightRadius;
        public float? needOutdoorsRefillRate;
        public CompProperties_GlassWall()
        {
            this.compClass = typeof(CompGlassWall);
        }
    }

    public class CompGlassWall : ThingComp
    {
        public CompProperties_GlassWall Props => base.props as CompProperties_GlassWall;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            var comp = parent.Map.GetComponent<MapComponent_Rebuild>();
            comp.glassWalls.Add(this);
            comp.BuildCellsCache();
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            var comp = map.GetComponent<MapComponent_Rebuild>();
            comp.glassWalls.Remove(this);
            comp.BuildCellsCache();
        }
    }
}
