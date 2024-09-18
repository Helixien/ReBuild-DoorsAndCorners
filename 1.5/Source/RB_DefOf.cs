using RimWorld;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [DefOf]
    public static class RB_DefOf
    {
        [MayRequireAnomaly] public static ThingDef RB_LargeSecurityDoor;
        public static ThingDef RB_OverwallArmor, RB_OverwallFireplace;
        public static ThingDef RB_FloorLamp;
        [MayRequire("VanillaExpanded.Temperature")] public static ThingDef RB_ACUnitFloor;
    }
}
