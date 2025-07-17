using HarmonyLib;
using RimWorld;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(SectionLayer_GravshipHull), "EnsureInitialized")]
    public static class SectionLayer_GravshipHull_EnsureInitialized_Patch
    {
        public static bool Prefix()
        {
            var extension = ThingDefOf.GravshipHull.GetModExtension<ThingExtension>();
            if (extension is null || extension.gravshipHullReplacement is null)
            {
                return true;
            }

            if (!SectionLayer_GravshipHull.initalized)
            {
                SectionLayer_GravshipHull.initalized = true;
                SectionLayer_GravshipHull.mat_Corner_NW = new CachedMaterial(extension.gravshipHullReplacement.Replace("Things/Building/Linked/GravshipHull/AngledGravshipHull_northwest"), SectionLayer_GravshipHull.WallShader);
                SectionLayer_GravshipHull.mat_Corner_NE = new CachedMaterial(extension.gravshipHullReplacement.Replace("Things/Building/Linked/GravshipHull/AngledGravshipHull_northeast"), SectionLayer_GravshipHull.WallShader);
                SectionLayer_GravshipHull.mat_Corner_SW = new CachedMaterial(extension.gravshipHullReplacement.Replace("Things/Building/Linked/GravshipHull/AngledGravshipHull_southwest"), SectionLayer_GravshipHull.WallShader);
                SectionLayer_GravshipHull.mat_Corner_SE = new CachedMaterial(extension.gravshipHullReplacement.Replace("Things/Building/Linked/GravshipHull/AngledGravshipHull_southeast"), SectionLayer_GravshipHull.WallShader);
                SectionLayer_GravshipHull.mat_Diagonal_NW = new CachedMaterial(extension.gravshipHullReplacement.Replace("Things/Building/Linked/GravshipHull/AngledGravshipHull_Partial_northwest"), SectionLayer_GravshipHull.WallShader);
                SectionLayer_GravshipHull.mat_Diagonal_NE = new CachedMaterial(extension.gravshipHullReplacement.Replace("Things/Building/Linked/GravshipHull/AngledGravshipHull_Partial_northeast"), SectionLayer_GravshipHull.WallShader);
                SectionLayer_GravshipHull.mat_Diagonal_SW = new CachedMaterial(extension.gravshipHullReplacement.Replace("Things/Building/Linked/GravshipHull/AngledGravshipHull_Partial_southwest"), SectionLayer_GravshipHull.WallShader);
                SectionLayer_GravshipHull.mat_Diagonal_SE = new CachedMaterial(extension.gravshipHullReplacement.Replace("Things/Building/Linked/GravshipHull/AngledGravshipHull_Partial_southeast"), SectionLayer_GravshipHull.WallShader);
                SectionLayer_GravshipHull.mat_SubStructure_W = new CachedMaterial("Things/Building/Linked/GravshipHull/SubstructureCorner_Full_west", SectionLayer_GravshipHull.SubstructureShader);
                SectionLayer_GravshipHull.mat_SubStructure_E = new CachedMaterial("Things/Building/Linked/GravshipHull/SubstructureCorner_Full_east", SectionLayer_GravshipHull.SubstructureShader);
                SectionLayer_GravshipHull.mat_SubStructureExtra_W = new CachedMaterial("Things/Building/Linked/GravshipHull/SubstructureCorner_Tip_west", SectionLayer_GravshipHull.SubstructureShader);
                SectionLayer_GravshipHull.mat_SubStructureExtra_E = new CachedMaterial("Things/Building/Linked/GravshipHull/SubstructureCorner_Tip_east", SectionLayer_GravshipHull.SubstructureShader);
            }
            return false;
        }

    }
}
