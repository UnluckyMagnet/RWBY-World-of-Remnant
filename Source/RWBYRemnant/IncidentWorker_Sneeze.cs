using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RWBYRemnant
{
    class IncidentWorker_Sneeze : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            List<Thing> dustPowderList = map.GetDirectlyHeldThings().ToList().FindAll(s => s.def.defName.Contains("DustPowder"));
            List<Pawn> pawns = map.mapPawns.AllPawns.ToList().FindAll(p => p.RaceProps.Humanlike);

            foreach (Thing dustPowder in dustPowderList)
            {
                if (pawns.Any(p => p.AdjacentTo8WayOrInside(dustPowder))) return true;               
            }
            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            List<Pawn> pawns = map.mapPawns.AllPawns.ToList().FindAll(p => p.RaceProps.Humanlike);
            List<Thing> dustPowderList = map.GetDirectlyHeldThings().ToList().FindAll(s => s.def.defName.Contains("DustPowder"));
            dustPowderList.Shuffle();

            if (Rand.Chance(0.5f))
            {
                foreach (Thing dustPowder in dustPowderList)
                {
                    Pawn pawn = pawns.FindAll(p => p.AdjacentTo8WayOrInside(dustPowder)).RandomElement();
                    dustPowder.TryGetComp<CompExplosive>().StartWick();
                    string labelExplosion = "LetterLabelSneezeExplosion".Translate().Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN").CapitalizeFirst();
                    string textExplosion = "LetterTextSneezeExplosion".Translate().Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN").CapitalizeFirst();
                    Find.LetterStack.ReceiveLetter(labelExplosion, textExplosion, LetterDefOf.NegativeEvent, pawn);
                    return true;
                }
            }
            else
            {
                foreach (Thing dustPowder in dustPowderList)
                {
                    Pawn pawn = pawns.FindAll(p => !p.AdjacentTo8WayOrInside(dustPowder)).RandomElement();
                    string label = "LetterLabelSneeze".Translate().Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN").CapitalizeFirst();
                    string text = "LetterTextSneeze".Translate().Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN").CapitalizeFirst();
                    Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent, pawn);
                    return true;
                }
            }            
            return false;
        }
    }
}
