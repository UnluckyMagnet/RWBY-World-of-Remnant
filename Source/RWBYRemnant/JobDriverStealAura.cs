using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RWBYRemnant
{
    public class JobDriverStealAura : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(TargetIndex.A), job);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            Toil reserveTargetA = Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
            yield return reserveTargetA;
            Toil toilGoto = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell).FailOnSomeonePhysicallyInteracting(TargetIndex.A).FailOnNotDowned(TargetIndex.A);
            yield return toilGoto;
            job.count = 1;

            Toil stealAuraPreparations = new Toil().FailOnSomeonePhysicallyInteracting(TargetIndex.A).FailOnNotDowned(TargetIndex.A);
            stealAuraPreparations.initAction = delegate ()
            {
                SoundDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(stealAuraPreparations.actor.Position, stealAuraPreparations.actor.Map, false));
                totalNeededWork = GenTicks.SecondsToTicks(10);
                workLeft = totalNeededWork;
            };
            stealAuraPreparations.tickAction = delegate ()
            {
                workLeft--;
                if (workLeft <= 0f)
                {
                    stealAuraPreparations.actor.jobs.curDriver.ReadyForNextToil();
                }
            };
            stealAuraPreparations.defaultCompleteMode = ToilCompleteMode.Never;
            stealAuraPreparations.WithProgressBar(TargetIndex.A, () => 1f - this.workLeft / this.totalNeededWork, false, -0.5f);
            yield return stealAuraPreparations;

            Toil stealAura = new Toil();
            stealAura.initAction = delegate ()
            {
                if (TargetA.Thing is Pawn targetPawn && targetPawn.RaceProps.Humanlike && TargetB.Thing is ThingWithComps glove)
                {
                    if (targetPawn.TryGetComp<CompAbilityUserAura>() != null && glove.TryGetComp<CompStealAura>() != null)
                    {
                        Hediff hediffAuraStolen = new Hediff();
                        hediffAuraStolen = HediffMaker.MakeHediff(RWBYDefOf.RWBY_AuraStolen, targetPawn);
                        targetPawn.health.AddHediff(hediffAuraStolen);

                        RWBYDefOf.AuraBreak.PlayOneShot(new TargetInfo(targetPawn.Position, targetPawn.Map, false));
                        if (stealAura.actor.Faction != targetPawn.Faction) stealAura.actor.Faction.TryAffectGoodwillWith(targetPawn.Faction, -15, true, true, /* "MessageFactionStoleAura".Translate(stealAura.actor.LabelShort, targetPawn.LabelShort),*/ lookTarget: stealAura.actor);
                        Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(RWBYDefOf.RWBY_AuraStolen_Relation);
                        targetPawn.needs.mood.thoughts.memories.TryGainMemory(thought_Memory, stealAura.actor);
                        glove.TryGetComp<CompStealAura>().stolenTraitDef = targetPawn.story.traits.allTraits.FindAll(t => t.def == RWBYDefOf.RWBY_Aura || SemblanceUtility.semblanceList.Contains(t.def)).RandomElement().def;
                        targetPawn.story.traits.allTraits.RemoveAll(t => t.def == RWBYDefOf.RWBY_Aura || SemblanceUtility.semblanceList.Contains(t.def));
                    }
                }
            };
            yield return stealAura;

            yield break;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref workLeft, "workLeft", 0f, false);
            Scribe_Values.Look<float>(ref totalNeededWork, "totalNeededWork", 0f, false);
        }

        private float workLeft;
        private float totalNeededWork;
    }
}
