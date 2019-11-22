using AbilityUser;
using RimWorld;
using UnityEngine;
using Verse;

namespace RWBYRemnant
{
    class Verb_AbilityIgnorePain : Verb_UseAbilitySemblanceBase
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
                    Hediff hediffIgnorePain = new Hediff();
                    hediffIgnorePain = HediffMaker.MakeHediff(RWBYDefOf.RWBY_IgnorePain, CasterPawn);
                    CasterPawn.health.AddHediff(hediffIgnorePain);
                    bool abilitySucceeded = true;
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
