using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(ImpactSoundUtility), "PlayImpactSound")]
    public static class ImpactSoundUtility_PlayImpactSound_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var impactBulletField = AccessTools.Field(typeof(StuffProperties), "soundImpactBullet");
            var impactMeleeField = AccessTools.Field(typeof(StuffProperties), "soundImpactMelee");
            foreach (var codeInstruction in codeInstructions)
            {
                yield return codeInstruction;
                if (codeInstruction.LoadsField(impactBulletField))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(ImpactSoundUtility_PlayImpactSound_Patch), "TryOverrideImpactBulletSound"));
                }
                if (codeInstruction.LoadsField(impactMeleeField))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(ImpactSoundUtility_PlayImpactSound_Patch), "TryOverrideImpactMeleeSound"));
                }
            }
        }

        public static SoundDef TryOverrideImpactBulletSound(SoundDef def, Thing hitThing)
        {
            var extension = hitThing.def.GetModExtension<ThingExtension>();
            if (extension?.soundImpactBulletOverride != null)
            {
                return extension.soundImpactBulletOverride;
            }
            return def;
        }

        public static SoundDef TryOverrideImpactMeleeSound(SoundDef def, Thing hitThing)
        {
            var extension = hitThing.def.GetModExtension<ThingExtension>();
            if (extension?.soundImpactMeleeOverride != null)
            {
                return extension.soundImpactMeleeOverride;
            }
            return def;
        }
    }
}
