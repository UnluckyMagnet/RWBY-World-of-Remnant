using AbilityUser;
using RimWorld;
using UnityEngine;
using Verse;

namespace RWBYRemnant
{
    class Verb_AbilityMagneticPulse : Verb_UseAbilitySemblanceBase
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
                    foreach (Thing thing in CasterPawn.Map.spawnedThings)
                    {
                        if (CasterPawn.Position.DistanceTo(thing.Position) <= this.verbProps.range && SemblanceUtility.PyrrhaMagnetismCanAffect(thing))
                        {
                            if (thing is Pawn pawn && !pawn.HostileTo(CasterPawn))
                            {
                                Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(RWBYDefOf.RWBY_PyrrhaHurtFriendly);
                                pawn.needs.mood.thoughts.memories.TryGainMemory(thought_Memory);
                            }
                            DamageInfo dinfo1 = new DamageInfo(DamageDefOf.EMP, 1f, 0f, -1f, CasterPawn, null, CasterPawn.def, DamageInfo.SourceCategory.ThingOrUnknown, thing);
                            dinfo1.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                            Vector3 direction1 = (thing.Position - CasterPawn.Position).ToVector3();
                            dinfo1.SetAngle(direction1);
                            thing.TakeDamage(dinfo1);

                            DamageInfo dinfo2 = new DamageInfo(DamageDefOf.Blunt, 200f, 0f, -1f, CasterPawn, null, CasterPawn.def, DamageInfo.SourceCategory.ThingOrUnknown, thing);
                            dinfo2.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                            Vector3 direction2 = (thing.Position - CasterPawn.Position).ToVector3();
                            dinfo2.SetAngle(direction2);
                            thing.TakeDamage(dinfo2);
                        }
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
