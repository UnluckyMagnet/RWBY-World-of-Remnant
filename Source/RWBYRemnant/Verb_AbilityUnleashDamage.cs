using AbilityUser;
using RimWorld;
using UnityEngine;
using Verse;

namespace RWBYRemnant
{
    class Verb_AbilityUnleashDamage : Verb_UseAbilitySemblanceBase
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
                    if (TargetsAoE[i].Thing != null && TargetsAoE[i].Thing is Pawn targetPawn)
                    {
                        float damageToDeal = ((Aura_Adam)CasterPawn.TryGetComp<CompAbilityUserAura>().aura).absorbedDamage;
                        if (targetPawn.TryGetComp<CompAbilityUserAura>() is CompAbilityUserAura compAbilityUserAura && targetPawn.TryGetComp<CompAbilityUserAura>().Initialized && compAbilityUserAura.aura.currentEnergy > 0f)
                        {
                            float damageToDealToAura = 0f;
                            if (compAbilityUserAura.aura.currentEnergy > damageToDeal)
                            {
                                damageToDealToAura = damageToDeal;
                                damageToDeal = 0;
                            }
                            else
                            {
                                damageToDealToAura = compAbilityUserAura.aura.currentEnergy * 100f + 1f;
                                damageToDeal = damageToDeal - compAbilityUserAura.aura.currentEnergy;
                            }
                            DamageInfo extraDinfoToAura = new DamageInfo(DamageDefOf.Cut, damageToDealToAura, 0f, -1f, CasterPawn, null, CasterPawn.def, DamageInfo.SourceCategory.ThingOrUnknown, targetPawn);
                            extraDinfoToAura.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                            Vector3 direction = (targetPawn.Position - CasterPawn.Position).ToVector3();
                            extraDinfoToAura.SetAngle(direction);
                            targetPawn.TakeDamage(extraDinfoToAura);
                        }
                        if (damageToDeal > 0)
                        {
                            DamageInfo extraDinfo = new DamageInfo(DamageDefOf.Cut, damageToDeal, 0f, -1f, CasterPawn, null, CasterPawn.def, DamageInfo.SourceCategory.ThingOrUnknown, targetPawn);
                            extraDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                            Vector3 direction = (targetPawn.Position - CasterPawn.Position).ToVector3();
                            extraDinfo.SetAngle(direction);
                            targetPawn.TakeDamage(extraDinfo);
                        }
                        Projectile projectile = (Projectile)GenSpawn.Spawn(RWBYDefOf.RWBY_Ability_Adam_Projectile, CasterPawn.Position, CasterPawn.Map, WipeMode.Vanish);
                        projectile.Launch(CasterPawn, targetPawn, targetPawn, ProjectileHitFlags.IntendedTarget);
                        ((Aura_Adam)CasterPawn.TryGetComp<CompAbilityUserAura>().aura).absorbedDamage = 1f;
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
