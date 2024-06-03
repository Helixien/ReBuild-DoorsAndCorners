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
            var curLevel = AccessTools.Method(typeof(RestUtility), nameof(RestUtility.InBed));
            foreach (var instruction in codeInstructions)
            {
                yield return instruction;
                if (patched is false && instruction.Calls(curLevel))
                {
                    patched = true;
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 1);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(Need_Outdoors_NeedInterval_Patch), "TryOverrideNeed"));
                }
            }
        }

        public static bool TryOverrideNeed(bool result, ref float value, Need_Outdoors need)
        {
            if (need.pawn.GetRoom() is Room room && room.UsesOutdoorTemperature is false)
            {
                var comp = need.pawn.Map.GetComponent<MapComponent_Rebuild>();
                if (room.BorderCells.Any(x => comp.cellsNearbyGlassWalls.Contains(x)))
                {
                    value = 1.6f;
                }
            }
            return result;
        }
    }
}
