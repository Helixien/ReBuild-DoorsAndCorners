using Verse;

namespace ReBuildDoorsAndCorners
{
	public class ReplaceTo
	{
		public string replace;
		public string with;

		public string Replace(string text)
		{
			return text.Replace(replace, with);
		}
	}

	public class ThingExtension : DefModExtension
	{
		public SoundDef soundImpactMeleeOverride, soundImpactBulletOverride;

		public bool closesRooms;

		public bool prisonersCannotOpenIt;

		public ReplaceTo gravshipHullReplacement;
	}
}
