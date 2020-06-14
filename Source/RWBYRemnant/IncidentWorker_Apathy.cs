using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RWBYRemnant
{
    public class IncidentWorker_Apathy : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            Pawn pawn = PawnGenerator.GeneratePawn(RWBYDefOf.Grimm_Apathy, FactionUtility.DefaultFactionFrom(RWBYDefOf.Creatures_of_Grimm));
            if (!parms.spawnCenter.IsValid && !RCellFinder.TryFindRandomPawnEntryCell(out parms.spawnCenter, map, CellFinder.EdgeRoadChance_Hostile, false, null))
            {
                return false;
            }
            parms.spawnRotation = Rot4.FromAngleFlat((map.Center - parms.spawnCenter).AngleFlat);
            IntVec3 loc = CellFinder.RandomClosewalkCellNear(parms.spawnCenter, map, 8, null);
            GenSpawn.Spawn(pawn, loc, map, parms.spawnRotation, WipeMode.Vanish, false);
            pawn.SetFaction(null);
            foreach (Pawn humanlike in map.mapPawns.AllPawnsSpawned.FindAll(p => p.RaceProps.Humanlike))
            {
                Hediff apathyTiredness = new Hediff();
                apathyTiredness = HediffMaker.MakeHediff(RWBYDefOf.RWBY_ApathyTiredness, humanlike);
                humanlike.health.AddHediff(apathyTiredness);
            }
            string label = "LetterLabelApathy".Translate();
            string text = "LetterTextApathy".Translate();
            List<Pawn> pawns = map.mapPawns.AllPawnsSpawned.FindAll(p => p.Faction == Faction.OfPlayer && p.RaceProps.Humanlike);
            if (pawns.Count > 0)
            {
                Pawn knowingPawn = pawns.RandomElement();
                text = "LetterTextApathyKnowing".Translate().Formatted(knowingPawn.Named("PAWN")).AdjustedFor(knowingPawn, "PAWN").CapitalizeFirst();
                Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.ThreatSmall, knowingPawn);
            }
            else
            {
                Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.ThreatSmall);
            }
            return true;
        }
    }
}
