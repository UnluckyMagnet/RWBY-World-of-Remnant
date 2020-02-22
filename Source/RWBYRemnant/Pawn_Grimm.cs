using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;

namespace RWBYRemnant
{
    class Pawn_Grimm : Pawn
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (attractGrimmTimer <= 0) this.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent, null, true, false, null, true);
        }

        public override void Tick()
        {
            base.Tick();
            if (Downed) Kill(null);
            attractGrimmTimer -= 1;
            if (attractGrimmTimer == 0)
            {
                IncidentDef localDef = IncidentDefOf.RaidEnemy;
                IncidentParms parms = StorytellerUtility.DefaultParmsNow(localDef.category, Map);
                StorytellerComp storytellerComp = Find.Storyteller.storytellerComps.First((StorytellerComp x) => x is StorytellerComp_OnOffCycle || x is StorytellerComp_RandomMain);
                parms = storytellerComp.GenerateParms(localDef.category, parms.target);
                parms.faction = FactionUtility.DefaultFactionFrom(RWBYDefOf.Creatures_of_Grimm);
                parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                localDef.Worker.TryExecute(parms);
                this.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent, null, true, false, null, true);
            }
        }

        public override void TickRare()
        {
            base.TickRare();
            if (this.CurJob.targetA.Thing is Pawn pawnA && pawnA.health.hediffSet.HasHediff(RWBYDefOf.RWBY_MaskedEmotions))
            {
                jobs.EndCurrentJob(JobCondition.InterruptForced, true);
            }
            if (this.CurJob.targetB.Thing is Pawn pawnB && pawnB.health.hediffSet.HasHediff(RWBYDefOf.RWBY_MaskedEmotions))
            {
                jobs.EndCurrentJob(JobCondition.InterruptForced, true);
            }
            if (this.CurJob.targetC.Thing is Pawn pawnC && pawnC.health.hediffSet.HasHediff(RWBYDefOf.RWBY_MaskedEmotions))
            {
                jobs.EndCurrentJob(JobCondition.InterruptForced, true);
            }
        }

        public void SetAttractGrimmTimer()
        {
            attractGrimmTimer = GenTicks.SecondsToTicks(Rand.RangeInclusive(60, 120));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref attractGrimmTimer, "attractGrimmTimer", -9999, false);
        }

        private int attractGrimmTimer = -9999;
    }
}
