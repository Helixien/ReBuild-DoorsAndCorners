using HarmonyLib;
using RimWorld;
using UnityEngine;
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
            var field = Traverse.Create(typeof(Building_MultiTileDoor)).Field("MoverDrawScale");
            var vec = field.GetValue<Vector3>();
            field.SetValue(new Vector3(vec.x + 0.03f, vec.y, vec.z));
        }
    }
}
