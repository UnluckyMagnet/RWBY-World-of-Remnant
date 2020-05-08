using AbilityUser;
using RimWorld;
using UnityEngine;
using Verse;

namespace RWBYRemnant
{
    class Verb_AbilityUseSilverEyes : Verb_UseAbilitySemblanceBase
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
                    foreach (Pawn targetPawn in CasterPawn.Map.mapPawns.AllPawnsSpawned)
                    {
                        if (GrimmUtility.IsGrimm(targetPawn) && CasterPawn.Position.DistanceTo(targetPawn.Position) <= this.verbProps.range)
                        {
                            ShootLine shootLine;
                            if (TryFindShootLineFromTo(CasterPawn.Position, targetPawn, out shootLine))
                            {
                                Projectile projectile = (Projectile)GenSpawn.Spawn(RWBYDefOf.RWBY_SilverEyes_Projectile, CasterPawn.Position, CasterPawn.Map);
                                projectile.Launch(CasterPawn, targetPawn, targetPawn, ProjectileHitFlags.IntendedTarget);
                            }
                        }
                    }

                    if (CasterPawn.story.traits.HasTrait(RWBYDefOf.RWBY_Aura))
                    {
                        Hediff silverEyesExhaustion = new Hediff();
                        silverEyesExhaustion = HediffMaker.MakeHediff(RWBYDefOf.RWBY_SilverEye_Exhaustion, CasterPawn);
                        if (burstShotsLeft == 1)
                        {
                            silverEyesExhaustion.Severity = 1.2f;
                        }
                        CasterPawn.health.AddHediff(silverEyesExhaustion);
                    }
                    
                    if (!CasterPawn.Downed)
                    {
                        MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(RWBYDefOf.RWBY_SilverEye_Mote, null);
                        moteThrown.Scale = 3f;
                        moteThrown.rotationRate = 0f;
                        moteThrown.exactPosition = CasterPawn.Drawer.DrawPos + CasterPawn.Drawer.renderer.BaseHeadOffsetAt(CasterPawn.Rotation);
                        moteThrown.SetVelocity(0f, 0f);
                        Log.Warning(moteThrown.exactPosition.ToString());
                        GenSpawn.Spawn(moteThrown, CasterPawn.Position, CasterPawn.Map, WipeMode.Vanish);
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
