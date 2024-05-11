using HarmonyLib;
using Verse;

namespace ReBuildDoorsAndCorners
{
    public class ReBuildDoorsAndCornersMod : Mod
    {
        public ReBuildDoorsAndCornersMod(ModContentPack pack) : base(pack)
        {
            new Harmony("ReBuildDoorsAndCornersMod").PatchAll();
        }
    }
}
