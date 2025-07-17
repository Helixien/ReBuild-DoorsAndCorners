using HarmonyLib;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(Building), nameof(Building.SpawnSetup))]
    public static class Building_SpawnSetup_Patch
    {
        public static void Postfix(Building __instance)
        {
            var extension = __instance.def.GetModExtension<ThingExtension>();
            if (extension is null || extension.spaceGraphicPath.NullOrEmpty())
            {
                return;
            }
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                Graphic graphic = __instance.Graphic;
                if (__instance.Map.Tile.LayerDef.isSpace)
                {
                    if (graphic.data.texPath != extension.spaceGraphicPath)
                    {
                        var copy = new GraphicData();
                        copy.CopyFrom(graphic.data);
                        copy.texPath = extension.spaceGraphicPath;
                        __instance.graphicInt = copy.GraphicColoredFor(__instance);
                    }
                }
                else
                {
                    if (graphic.data.texPath == extension.spaceGraphicPath)
                    {
                        var copy = new GraphicData();
                        copy.CopyFrom(graphic.data);
                        copy.texPath = __instance.def.graphicData.texPath;
                        __instance.graphicInt = copy.GraphicColoredFor(__instance);
                    }
                }

                var map = __instance.Map;
                __instance.DirtyMapMesh(map);
            });
        }
    }
}
