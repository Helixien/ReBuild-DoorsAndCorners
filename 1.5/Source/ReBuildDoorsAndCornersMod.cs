using HarmonyLib;
using Verse;

namespace ReBuildDoorsAndCorners
{
    public class ReBuildDoorsAndCornersMod : Mod
    {
        public static ModContentPack modInstance;
        public const string RB_ColonistsDontDropItemsAtDoors = "RB_ColonistsDontDropItemsAtDoors";
        public const string RB_EntitiesCannotOpenSecurityDoors = "RB_EntitiesCannotOpenSecurityDoors";
        public ReBuildDoorsAndCornersMod(ModContentPack pack) : base(pack)
        {
            new Harmony("ReBuildDoorsAndCornersMod").PatchAll();
            modInstance = pack;
        }
    }
}
