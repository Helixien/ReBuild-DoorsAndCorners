using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(Building_MultiTileDoor), "DrawAt")]
    public static class Building_MultiTileDoor_DrawAt_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var getGraphic = AccessTools.Method(typeof(GraphicData), "get_Graphic");
            var field = AccessTools.Field(typeof(BuildingProperties), "upperMoverGraphic");
            var codes = codeInstructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                yield return code;
                if (code.Calls(getGraphic) && codes[i - 1].LoadsField(field))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(Building_MultiTileDoor_DrawAt_Patch), "TryOverrideGraphic"));
                }
            }
        }

        public static Graphic TryOverrideGraphic(Graphic graphic, Building_MultiTileDoor door)
        {
            if (door is Building_MultiTileDoorStuffColor stuff)
            {
                return stuff.UpperMoverGraphic;
            }
            return graphic;
        }
    }
}
