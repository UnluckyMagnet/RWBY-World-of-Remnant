using RimWorld;
using System.Linq;
using Verse;

namespace RWBYRemnant
{
    public class CompProperties_TimeDilationGlyph : CompProperties
    {
        public CompProperties_TimeDilationGlyph()
        {
            compClass = typeof(CompTimeDilationGlyph);
        }
    }

    public class CompTimeDilationGlyph : ThingComp
    {
        public CompProperties_TimeDilationGlyph Props => (CompProperties_TimeDilationGlyph)props;

        public int ticksAlive;

        public void SetData(int tmpTicksToSummon)
        {
            ticksAlive = tmpTicksToSummon;
        }

        public override void CompTick()
        {
            foreach (Pawn pawn in parent.Map.mapPawns.AllPawns)
            {
                if (pawn.AdjacentTo8WayOrInside(parent))
                {
                    Hediff hediff = HediffMaker.MakeHediff(RWBYDefOf.RWBY_TimeDilation, pawn);
                    pawn.health.AddHediff(hediff);
                }
            }
            ticksAlive--;
            if (ticksAlive < 1)
            {                
                parent.Destroy();
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref ticksAlive, "ticksAlive", 1, false);
        }
    }
}
