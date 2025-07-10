using HarmonyLib;
using Verse;

namespace ReBuildDoorsAndCorners
{
	[HotSwappable]
	[HarmonyPatch(typeof(RegionTypeUtility), "GetExpectedRegionType")]
	public static class RegionTypeUtility_GetExpectedRegionType_Patch
	{
		public static void Postfix(IntVec3 c, Map map, ref RegionType __result)
		{
			if (__result == RegionType.Set_Impassable)
			{
				var building = c.GetFirstBuilding(map);
				if (building != null)
				{
					var extension = building.def.GetModExtension<ThingExtension>();
					if (extension != null && extension.closesRooms)
					{
						__result = RegionType.None;
					}
				}
			}
		}
	}
}
