using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RWBYRemnant
{
    class IncidentWorker_Nuckelavee : IncidentWorker
    {
        public override float AdjustedChance => base.AdjustedChance * MoodChance() * QrowChance();

        public float MoodChance()
        {
            List<Map> maps = Find.Maps.FindAll(m => m.IsPlayerHome);
            float mood = 0f;
            int pawnCount = 0;
            foreach (Map map in maps)
            {
                List<Pawn> pawns = map.PlayerPawnsForStoryteller.ToList().FindAll(p => p.RaceProps.Humanlike && !p.health.hediffSet.HasHediff(RWBYDefOf.RWBY_MaskedEmotions));
                
                foreach (Pawn pawn in pawns)
                {
                    mood += pawn.needs.mood.CurLevelPercentage;
                    pawnCount++;
                }
            }
            float averageMood = mood / pawnCount;
            float averageMoodInverted = 1 - averageMood;
            return averageMoodInverted;
        }

        public float QrowChance()
        {
            List<Map> maps = Find.Maps.FindAll(m => m.IsPlayerHome);
            int pawnCount = 0;
            foreach (Map map in maps)
            {
                List<Pawn> pawns = map.PlayerPawnsForStoryteller.ToList().FindAll(p => p.RaceProps.Humanlike && p.story.traits.HasTrait(RWBYDefOf.Semblance_Qrow));

                foreach (Pawn pawn in pawns)
                {
                    pawnCount++;
                }
            }
            return Math.Max(1f, 1f + (pawnCount * 0.2f));
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            Pawn pawn = PawnGenerator.GeneratePawn(RWBYDefOf.Grimm_Nuckelavee, FactionUtility.DefaultFactionFrom(RWBYDefOf.Creatures_of_Grimm));
            if (!parms.spawnCenter.IsValid && !RCellFinder.TryFindRandomPawnEntryCell(out parms.spawnCenter, map, CellFinder.EdgeRoadChance_Hostile, false, null))
            {
                return false;
            }
            parms.spawnRotation = Rot4.FromAngleFlat((map.Center - parms.spawnCenter).AngleFlat);
            IntVec3 loc = CellFinder.RandomClosewalkCellNear(parms.spawnCenter, map, 8, null);
            GenSpawn.Spawn(pawn, loc, map, parms.spawnRotation, WipeMode.Vanish, false);
            string label = "LetterLabelNuckelavee".Translate();
            string text = "LetterTextNuckelavee".Translate();
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.ThreatBig, pawn);

            IncidentDef localDef = IncidentDefOf.RaidEnemy;
            IncidentParms parms2 = StorytellerUtility.DefaultParmsNow(localDef.category, (Map)parms.target);
            StorytellerComp storytellerComp = Find.Storyteller.storytellerComps.First((StorytellerComp x) => x is StorytellerComp_OnOffCycle || x is StorytellerComp_RandomMain);
            parms2 = storytellerComp.GenerateParms(localDef.category, parms2.target);
            parms2.faction = FactionUtility.DefaultFactionFrom(RWBYDefOf.Creatures_of_Grimm);
            parms2.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
            parms2.points *= 0.6f;
            localDef.Worker.TryExecute(parms2);
            return true;
        }
    }
}
