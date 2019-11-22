using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RWBYRemnant
{
    class IncidentWorker_UnlockAura : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().auraUnlockable) return false;
            Map map = (Map)parms.target;
            List<Pawn> injuredPawns = map.PlayerPawnsForStoryteller.ToList().FindAll(p => p.RaceProps.Humanlike && p.health.hediffSet.hediffs.Any(h => h.GetType().Equals(typeof(Hediff_Injury)) && ((Hediff_Injury)h).TendableNow(true)));
            injuredPawns.RemoveAll(p => p.health.hediffSet.HasHediff(RWBYDefOf.RWBY_AuraStolen));
            if (injuredPawns.Count() == 0) return false;
            List<Pawn> pawnsWithoutAura = injuredPawns.FindAll(p => !p.story.traits.HasTrait(RWBYDefOf.RWBY_Aura) && !SemblanceUtility.semblanceList.Any(s => p.story.traits.HasTrait(s)));
            return pawnsWithoutAura.Count > 0;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            List<Pawn> injuredPawns = map.PlayerPawnsForStoryteller.ToList().FindAll(p => p.RaceProps.Humanlike && p.health.hediffSet.hediffs.Any(a => a.GetType().Equals(typeof(Hediff_Injury)) && ((Hediff_Injury)a).TendableNow(true)));
            injuredPawns.RemoveAll(p => p.health.hediffSet.HasHediff(RWBYDefOf.RWBY_AuraStolen));
            List<Pawn> pawnsWithoutAura = injuredPawns.FindAll(p => !p.story.traits.HasTrait(RWBYDefOf.RWBY_Aura) && !SemblanceUtility.semblanceList.Any(s => p.story.traits.HasTrait(s)));
            if (pawnsWithoutAura.Count == 0) return false;
            Pawn pawnWithoutAura = pawnsWithoutAura.RandomElement();
            return SemblanceUtility.UnlockAura(pawnWithoutAura, "LetterTextUnlockAuraHurt");
        }
    }
}
