using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(StatWorker_ContainmentStrength), "CalculateValues")]
    public static class StatWorker_ContainmentStrength_CalculateValues_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeInstruction lastInstruction = null;
            var found = false;
            foreach (var instruction in instructions)
            {
                if (!found
                    && instruction?.opcode == OpCodes.Stloc_S
                    && (instruction.operand as LocalBuilder)?.LocalIndex == 4
                    && lastInstruction?.opcode == OpCodes.Add)
                {
                    found = true;
                    yield return CodeInstruction.LoadLocal(24);
                    yield return CodeInstruction.LoadLocal(0);
                    yield return new CodeInstruction(OpCodes.Call, 
                        AccessTools.Method(typeof(StatWorker_ContainmentStrength_CalculateValues_Patch), "GetWallArmorHP"));
                    yield return new CodeInstruction(OpCodes.Add);
                }

                lastInstruction = instruction;
                yield return instruction;
            }
        }

        private static float GetWallArmorHP(IntVec3 item, Map map)
        {
            var thingList = item.GetThingList(map).Where(thing => thing.def == RB_DefOf.RB_OverwallArmor);
            foreach (var thing in thingList)
            {
                if (thing is Building building)
                {
                    return building.HitPoints;
                }
            }

            return 0f;
        }
    }
}
