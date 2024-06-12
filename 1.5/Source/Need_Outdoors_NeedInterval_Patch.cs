using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(Need_Outdoors), "NeedInterval")]
    public static class Need_Outdoors_NeedInterval_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            bool patched = false;
            var inBed = AccessTools.Method(typeof(RestUtility), nameof(RestUtility.InBed));
            foreach (var instruction in codeInstructions)
            {
                yield return instruction;
                if (patched is false && instruction.Calls(inBed))
                {
                    patched = true;
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 1);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(Need_Outdoors_NeedInterval_Patch), "TryOverrideNeed"));
                }
            }
        }

        public static bool TryOverrideNeed(bool result, RoofDef roof, ref float value, Need_Outdoors need)
        {
            if (roof != null && need.pawn.GetRoom() is Room room && room.PsychologicallyOutdoors is false)
            {
                var comp = need.pawn.Map?.GetComponent<MapComponent_Rebuild>();
                if (comp.glassWalls.Any())
                {
                    var roomThings = room.ContainedAndAdjacentThings.ToHashSet();
                    var wallGlass = comp.glassWalls.Where(x => roomThings.Contains(x.parent)
                        && x.Props.needOutdoorsRefillRate.HasValue).GroupBy(i => i.parent.def)
                        .OrderByDescending(g => g.Count()).FirstOrDefault()?.FirstOrDefault();
                    if (wallGlass != null)
                    {
                        value = wallGlass.Props.needOutdoorsRefillRate.Value;
                    }
                }
            }
            return result;
        }
    }
}
