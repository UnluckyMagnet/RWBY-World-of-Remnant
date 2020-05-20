using System.Linq;
using Verse;

namespace RWBYRemnant
{
    public class HediffCompProperties_ApathyTiredness : HediffCompProperties
    {
        public HediffCompProperties_ApathyTiredness()
        {
            compClass = typeof(HediffComp_ApathyTiredness);
        }
    }

    public class HediffComp_ApathyTiredness : HediffComp
    {
        public HediffCompProperties_ApathyTiredness Props => (HediffCompProperties_ApathyTiredness)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            bool apathyOnMap = false;
            if (this.Pawn.Map != null && this.Pawn.Map.mapPawns.AllPawnsSpawned.Any(p => p.RaceProps.AnyPawnKind == RWBYDefOf.Grimm_Apathy))
            {
                apathyOnMap = true;
            }

            if (apathyOnMap)
            {
                if (this.Pawn.IsHashIntervalTick(30000)) severityAdjustment += (0.1f * (1f - this.Pawn.needs.mood.CurInstantLevelPercentage));
            }
            else
            {
                severityAdjustment -= 1f;
            }
        }
    }
}
