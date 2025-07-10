using UnityEngine;
using Verse;
using System.Collections.Generic;

namespace ReBuildDoorsAndCorners
{
	public class CompProperties_AdditionalGraphic : CompProperties
	{
		public CompProperties_AdditionalGraphic()
		{
			this.compClass = typeof(CompAdditionalGraphic);
		}
		public GraphicData graphicData;
		public string toggleLabel;
		public string toggleDesc;
		public string toggleIcon;
	}

	[HotSwappable]
	public class CompAdditionalGraphic : ThingComp
	{
		public CompProperties_AdditionalGraphic Props => (CompProperties_AdditionalGraphic)props;

		private bool graphicsVisible = false;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref graphicsVisible, "graphicsVisible", false);
		}
		
		public override void PostDraw()
		{
			base.PostDraw();
			if (graphicsVisible)
			{
				Props.graphicData.Graphic.Draw(parent.DrawPos, parent.Rotation, parent);
			}
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				yield return gizmo;
			}

			yield return new Command_Toggle
			{
				defaultLabel = Props.toggleLabel,
				defaultDesc = Props.toggleDesc,
				icon = ContentFinder<Texture2D>.Get(Props.toggleIcon, true),
				isActive = () => graphicsVisible,
				toggleAction = () => graphicsVisible = !graphicsVisible
			};
		}
	}
}