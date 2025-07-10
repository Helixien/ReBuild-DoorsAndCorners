using RimWorld;
using Verse;

namespace ReBuildDoorsAndCorners
{
    public class CompFireplace : ThingComp
    {
        private const int HeatPushInterval = 60;

        public CompProperties_HeatPusher Props => (CompProperties_HeatPusher)props;

        protected CompRefuelable refuelableComp;

        public bool ShouldPushHeatNow
        {
            get
            {
                if (!parent.SpawnedOrAnyParentSpawned || refuelableComp != null && !refuelableComp.HasFuel)
                {
                    return false;
                }
                CompProperties_HeatPusher compProperties_HeatPusher = Props;
                float ambientTemperature = parent.AmbientTemperature;
                if (ambientTemperature < compProperties_HeatPusher.heatPushMaxTemperature)
                {
                    var shouldPush = ambientTemperature > compProperties_HeatPusher.heatPushMinTemperature;
                    return shouldPush;
                }
                return false;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            refuelableComp = parent.GetComp<CompRefuelable>();
        }

        public override void CompTick()
        {
            base.CompTick();
            if (parent.IsHashIntervalTick(60) && ShouldPushHeatNow)
            {
                var pos = parent.PositionHeld + IntVec3.North.RotatedBy(parent.Rotation);
                GenTemperature.PushHeat(pos, parent.MapHeld, Props.heatPerSecond);
            }
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            if (ShouldPushHeatNow)
            {
                var pos = parent.PositionHeld + IntVec3.North.RotatedBy(parent.Rotation);
                GenTemperature.PushHeat(pos, parent.MapHeld, Props.heatPerSecond * 4.1666665f);
            }
        }
    }
}
