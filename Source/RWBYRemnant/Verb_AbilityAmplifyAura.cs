using AbilityUser;
using RimWorld;
using Verse;

namespace RWBYRemnant
{
    class Verb_AbilityAmplifyAura : Verb_UseAbilitySemblanceBase
    {
        protected override bool TryCastShot()
        {
            bool flag = false;
            this.TargetsAoE.Clear();
            this.UpdateTargets();
            int shotsPerBurst = this.ShotsPerBurst;
            bool flag2 = this.UseAbilityProps.AbilityTargetCategory != AbilityTargetCategory.TargetAoE && this.TargetsAoE.Count > 1;
            if (flag2)
            {
                this.TargetsAoE.RemoveRange(0, this.TargetsAoE.Count - 1);
            }
            bool flag3 = this.UseAbilityProps.mustHaveTarget && this.TargetsAoE.Count == 0;
            bool result;
            if (flag3)
            {
                Messages.Message("AU_NoTargets".Translate(), MessageTypeDefOf.RejectInput, true);
                this.Ability.Notify_AbilityFailed(true);
                result = false;
            }
            else
            {
                for (int i = 0; i < this.TargetsAoE.Count; i++)
                {
                    bool abilitySucceeded = true;
                    if (TargetsAoE[i].Thing != null && TargetsAoE[i].Thing is Pawn targetPawn && targetPawn.TryGetComp<CompAbilityUserAura>() != null && targetPawn.TryGetComp<CompAbilityUserAura>().Initialized)
                    {
                        Hediff hediffAmplifiedAura = new Hediff();
                        hediffAmplifiedAura = HediffMaker.MakeHediff(RWBYDefOf.RWBY_AmplifiedAura, targetPawn);
                        targetPawn.health.AddHediff(hediffAmplifiedAura);
                        targetPawn.TryGetComp<CompAbilityUserAura>().aura.currentEnergy += 0.5f;
                        if (targetPawn.TryGetComp<CompAbilityUserAura>().aura.currentEnergy > targetPawn.TryGetComp<CompAbilityUserAura>().aura.maxEnergy) targetPawn.TryGetComp<CompAbilityUserAura>().aura.currentEnergy = targetPawn.TryGetComp<CompAbilityUserAura>().aura.maxEnergy;
                    }
                    else
                    {
                        abilitySucceeded = false;
                    }                    
                    if (abilitySucceeded) flag = true;
                }
                this.PostCastShot(flag, out flag);
                bool postSucceed = !flag;
                if (postSucceed)
                {
                    this.Ability.Notify_AbilityFailed(this.UseAbilityProps.refundsPointsAfterFailing);
                }
                result = flag;
            }
            return result;
        }
    }
}
