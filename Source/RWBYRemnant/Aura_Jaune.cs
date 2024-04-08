using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RWBYRemnant
{
    public class Aura_Jaune : Aura
    {
        public override bool TryAbsorbDamage(DamageInfo dinfo)
        {
            if (currentEnergy == 0f || dinfo.Def == DamageDefOf.SurgicalCut) return false;
            if (!pawn.Drafted && !pawn.IsFighting() && Rand.Chance(0.2f))
            {
                if (pawn.IsColonist)
                {
                    Messages.Message("MessageTextForgotAura".Translate().Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN").CapitalizeFirst(), pawn, MessageTypeDefOf.NegativeEvent);
                }
                return false;
            }

            if (pawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_AmplifiedAura))
            {
                currentEnergy -= dinfo.Amount / 200f; // amplified Aura takes half the damage
            }
            else
            {
                currentEnergy -= dinfo.Amount / 100f;
            }
            if (currentEnergy > 0f) // normal absorb
            {
                RWBYDefOf.AuraFlicker.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
                this.impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
                Vector3 loc = pawn.TrueCenter() + impactAngleVect.RotatedBy(180f) * 0.5f;
                float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
                FleckMaker.Static(loc, pawn.Map, FleckDefOf.ExplosionFlash, num);
                int num2 = (int)num;
                for (int i = 0; i < num2; i++)
                {
                    FleckMaker.ThrowDustPuff(loc, pawn.Map, Rand.Range(0.8f, 1.2f));
                }
                lastAbsorbDamageTick = Find.TickManager.TicksGame;
            }
            else // break
            {
                RWBYDefOf.AuraBreak.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
                FleckMaker.Static(pawn.TrueCenter(), pawn.Map, FleckDefOf.ExplosionFlash, 12f);
                for (int i = 0; i < 6; i++)
                {
                    Vector3 loc = pawn.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f);
                    FleckMaker.ThrowDustPuff(loc, pawn.Map, Rand.Range(0.8f, 1.2f));
                }
                currentEnergy = 0f;
            }

            return true;
        }

        public override Color GetColor()
        {
            return color;
        }

        public Color color = new Color(0.8f, 0.8f, 0.8f);
    }
}
