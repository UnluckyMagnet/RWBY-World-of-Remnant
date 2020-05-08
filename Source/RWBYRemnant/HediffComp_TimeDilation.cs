using System.Linq;
using Verse;

namespace RWBYRemnant
{
    public class HediffCompProperties_TimeDilationGlyph : HediffCompProperties
    {
        public HediffCompProperties_TimeDilationGlyph()
        {
            compClass = typeof(HediffComp_TimeDilationGlyph);
        }
    }

    public class HediffComp_TimeDilationGlyph : HediffComp
    {
        public HediffCompProperties_TimeDilationGlyph Props => (HediffCompProperties_TimeDilationGlyph)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);            
            bool isNearGlyph = false;
            if (this.Pawn.Map != null)
            {
                foreach (Thing thing in this.Pawn.Map.GetDirectlyHeldThings().ToList().FindAll(t => t.def == RWBYDefOf.Weiss_Glyph_TimeDilation))
                {
                    if (Pawn.AdjacentTo8WayOrInside(thing)) isNearGlyph = true;
                }
            }

            if (!isNearGlyph)
            {
                severityAdjustment -= 1f;
            }
        }
    }
}
