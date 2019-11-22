using AbilityUser;
using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RWBYRemnant
{
    public class Verb_UseAbilitySemblanceBase : Verb_UseAbility
    {
        public CompAbilityUserAura AbilityUserCompAura
        {
            get
            {
                return base.CasterPawn.TryGetComp<CompAbilityUserAura>();
            }
        }

        public override void PostCastShot(bool inResult, out bool outResult)
        {
            if (inResult)
            {
                if (burstShotsLeft == ShotsPerBurst)
                {
                    ConsumeAmmunition();
                    ConsumeAura();
                }
            }
            outResult = inResult;
        }

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
                    bool? abilitySucceeded = this.TryLaunchProjectile(this.verbProps.defaultProjectile, this.TargetsAoE[i]);
                    if (abilitySucceeded == true) flag = true;
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

        protected new bool? TryLaunchProjectile(ThingDef projectileDef, LocalTargetInfo launchTarget)
        {
            ShootLine shootLine;
            bool flag = base.TryFindShootLineFromTo(this.caster.Position, launchTarget, out shootLine);
            bool flag2 = this.verbProps.stopBurstWithoutLos && !flag;
            bool? result;
            if (flag2)
            {
                result = new bool?(false);
            }
            else
            {
                Vector3 drawPos = this.caster.DrawPos;
                Projectile projectile = (Projectile)GenSpawn.Spawn(projectileDef, shootLine.Source, this.caster.Map, WipeMode.Vanish);
                SoundDef soundCast = this.verbProps.soundCast;
                if (soundCast != null)
                {
                    soundCast.PlayOneShot(new TargetInfo(this.caster.Position, this.caster.Map, false));
                }
                SoundDef soundCastTail = this.verbProps.soundCastTail;
                if (soundCastTail != null)
                {
                    soundCastTail.PlayOneShotOnCamera(null);
                }
                bool drawShooting = DebugViewSettings.drawShooting;
                if (drawShooting)
                {
                    MoteMaker.ThrowText(this.caster.DrawPos, this.caster.Map, "ToHit", -1f);
                }
                ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.IntendedTarget;
                bool canHitNonTargetPawnsNow = this.canHitNonTargetPawnsNow;
                if (canHitNonTargetPawnsNow)
                {
                    projectileHitFlags |= ProjectileHitFlags.NonTargetPawns;
                }
                bool flag3 = !this.currentTarget.HasThing || this.currentTarget.Thing.def.Fillage == FillCategory.Full;
                if (flag3)
                {
                    projectileHitFlags |= ProjectileHitFlags.NonTargetWorld;
                }

                // miss chance
                ShotReport shotReport = ShotReport.HitReportFor(this.caster, this, this.currentTarget);
                Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
                ThingDef targetCoverDef = (randomCoverToMissInto == null) ? null : randomCoverToMissInto.def;
                if (!Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture) && !((VerbProperties_Ability)verbProps).AlwaysHits)
                {
                    shootLine.ChangeDestToMissWild(shotReport.AimOnTargetChance_StandardTarget);
                    ProjectileHitFlags projectileHitFlags2 = ProjectileHitFlags.NonTargetWorld;
                    if (Rand.Chance(0.5f) && this.canHitNonTargetPawnsNow)
                    {
                        projectileHitFlags2 |= ProjectileHitFlags.NonTargetPawns;
                    }
                    projectile.Launch(caster, drawPos, shootLine.Dest, this.currentTarget, projectileHitFlags2, null, targetCoverDef);
                    return true;
                }
                if (this.currentTarget.Thing != null && this.currentTarget.Thing.def.category == ThingCategory.Pawn && !Rand.Chance(shotReport.PassCoverChance) && !((VerbProperties_Ability)verbProps).AlwaysHits)
                {
                    ProjectileHitFlags projectileHitFlags3 = ProjectileHitFlags.NonTargetWorld;
                    if (this.canHitNonTargetPawnsNow)
                    {
                        projectileHitFlags3 |= ProjectileHitFlags.NonTargetPawns;
                    }
                    projectile.Launch(caster, drawPos, randomCoverToMissInto, this.currentTarget, projectileHitFlags3, null, targetCoverDef);
                    return true;
                }
                // miss chance end

                projectile.Launch(caster, launchTarget, launchTarget, projectileHitFlags, null);
                result = new bool?(true);
            }
            return result;
        }

        public bool ConsumeAmmunition()
        {
            SemblanceDef abilityDef_RWBY = ((AbilitySemblance)Ability).SemblanceDef;
            Thing thing = CasterPawn.inventory.GetDirectlyHeldThings().ToList().Find(s => s.def == abilityDef_RWBY.usesAmmunition);
            if (abilityDef_RWBY.usesAmmunition == null)
            {
                return true;
            }
            else if (abilityDef_RWBY.usesAmmunition != null && thing == null)
            {
                return false;
            }
            else
            {
                thing.stackCount = thing.stackCount - 1;
                if (thing.stackCount == 0) thing.Destroy();
                return true;
            }
        }

        public bool ConsumeAura()
        {
            SemblanceDef semblanceDef = ((AbilitySemblance)Ability).SemblanceDef;
            if (semblanceDef.auraCost == 0f)
            {
                return true;
            }
            else if (semblanceDef.auraCost > 0f && semblanceDef.auraCost >= AbilityUserCompAura.aura.currentEnergy)
            {
                return false;
            }
            else
            {
                return AbilityUserCompAura.aura.TryConsumeAura(semblanceDef.auraCost);
            }
        }
    }
}
