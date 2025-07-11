using HarmonyLib;
using RimWorld;
using Verse;

namespace ReBuildDoorsAndCorners
{
    [HarmonyPatch(typeof(Building_Door), "CanPhysicallyPass")]
	public static class Building_Door_CanPhysicallyPass_Patch
	{
		public static void Postfix(Building_Door __instance, ref bool __result, Pawn p)
		{
			if (__result)
			{
				var extension = __instance.def.GetModExtension<ThingExtension>();
				if (extension != null && extension.prisonersCannotOpenIt 
					&& (p.IsPrisoner || p.IsSlave) && p.HostileTo(__instance.Faction))
				{
					__result = false;
				}
            }
		}
	}
}
