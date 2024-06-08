using HarmonyLib;
using System.Diagnostics;
using Verse;
using Verse.Sound;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(SoundStarter), "PlayOneShot")]
    public static class SoundStarter_PlayOneShot_Patch
    {
        public static void Postfix(SoundDef soundDef, SoundInfo info)
        {
            Log.Message("Playing sound: " + soundDef + " - " + new StackTrace());
        }
    }

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
