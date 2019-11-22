using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;
using System.Linq;

namespace RWBYRemnant
{
    public class Aura : IExposable
    {
        public Aura()
        {
            maxEnergy = 1f;
            currentEnergy = 1f;
            pawn = null;
        }

        public virtual void Tick()
        {            
            if (!pawn.IsFighting() && Find.TickManager.TicksGame % GenTicks.SecondsToTicks(8) == 0) // every x seconds Aura can either heal or regenerate IF pawn is out of combat
            {
                List<Hediff_Injury> injuriesHealable = new List<Hediff_Injury>();
                foreach (Hediff hediff_Injury in pawn.health.hediffSet.hediffs)
                {
                    if (hediff_Injury.GetType().Equals(typeof(Hediff_Injury)) && ((Hediff_Injury)hediff_Injury).TendableNow(true))
                    {
                        injuriesHealable.Add((Hediff_Injury)hediff_Injury);
                    }
                }
                if (injuriesHealable.Count > 0 && TryConsumeAura(0.01f))
                {
                    injuriesHealable.RandomElement().Heal(1f);
                }
                else
                {
                    // injured pawns won´t regenerate aura if aura is empty, regenerates only out of combat and last taken damage is 30 seconds away
                    if (currentEnergy < maxEnergy && Find.TickManager.TicksGame - lastAbsorbDamageTick > GenTicks.SecondsToTicks(30f)) currentEnergy += 0.01f;
                }                
            }

            if (Find.TickManager.TicksGame % GenTicks.SecondsToTicks(1) == 0 && (pawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_AmplifiedAura) || pawn.health.hediffSet.hediffs.Any(h => SemblanceUtility.injectedDustCrystalHediffs.Contains(h.def)))) // amplified Aura heals faster
            {
                int healAmount = 0;
                if (pawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_AmplifiedAura))
                {
                    healAmount += 3;
                }
                if (pawn.health.hediffSet.hediffs.Any(h => SemblanceUtility.injectedDustCrystalHediffs.Contains(h.def)))
                {
                    healAmount += pawn.health.hediffSet.hediffs.FindAll(h => SemblanceUtility.injectedDustCrystalHediffs.Contains(h.def)).Sum(s => s.CurStageIndex + 1);
                }
                List<Hediff_Injury> injuriesHealable = new List<Hediff_Injury>();
                foreach (Hediff hediff_Injury in pawn.health.hediffSet.hediffs)
                {
                    if (hediff_Injury.GetType().Equals(typeof(Hediff_Injury)) && ((Hediff_Injury)hediff_Injury).TendableNow(true))
                    {
                        injuriesHealable.Add((Hediff_Injury)hediff_Injury);
                    }
                }
                if (injuriesHealable.Count > 0)
                {
                    for (int a = 0; a < healAmount; a++)
                    {
                        if (TryConsumeAura(0.01f)) injuriesHealable.RandomElement().Heal(1f);
                    }
                }
            }

            if (Find.TickManager.TicksGame % GenTicks.SecondsToTicks(0.5f) == 0 && pawn.health.hediffSet.hediffs.Any(h => SemblanceUtility.injectedDustCrystalHediffs.Contains(h.def))) // with Dust injected Aura regenerates faster
            {
                float energyToRegenerate = pawn.health.hediffSet.hediffs.FindAll(h => SemblanceUtility.injectedDustCrystalHediffs.Contains(h.def)).Sum(s => s.CurStageIndex + 1) / 100f;
                if (currentEnergy < maxEnergy) currentEnergy += energyToRegenerate;
            }

            if (currentEnergy > maxEnergy) currentEnergy = maxEnergy;
        }

        public virtual void TickRare()
        {

        }

        public virtual bool TryAbsorbDamage(DamageInfo dinfo)
        {
            if (currentEnergy == 0f || dinfo.Def == DamageDefOf.SurgicalCut) return false;
            if (!pawn.Drafted && !pawn.IsFighting() && Rand.Chance(0.05f))
            {
                if (pawn.IsColonist)
                {
                    Messages.Message("MessageTextForgotAura".Translate().Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN").CapitalizeFirst(), pawn, MessageTypeDefOf.NegativeEvent);
                }
                return false;
            }

            currentEnergy -= dinfo.Amount / 100f;
            if (currentEnergy > 0f) // normal absorb
            {
                RWBYDefOf.AuraFlicker.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
                this.impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
                Vector3 loc = pawn.TrueCenter() + impactAngleVect.RotatedBy(180f) * 0.5f;
                float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
                MoteMaker.MakeStaticMote(loc, pawn.Map, ThingDefOf.Mote_ExplosionFlash, num);
                int num2 = (int)num;
                for (int i = 0; i < num2; i++)
                {
                    MoteMaker.ThrowDustPuff(loc, pawn.Map, Rand.Range(0.8f, 1.2f));
                }
                lastAbsorbDamageTick = Find.TickManager.TicksGame;
            }
            else // break
            {
                RWBYDefOf.AuraBreak.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
                MoteMaker.MakeStaticMote(pawn.TrueCenter(), pawn.Map, ThingDefOf.Mote_ExplosionFlash, 12f);
                for (int i = 0; i < 6; i++)
                {
                    Vector3 loc = pawn.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f);
                    MoteMaker.ThrowDustPuff(loc, pawn.Map, Rand.Range(0.8f, 1.2f));
                }
                currentEnergy = 0f;
            }

            return true;
        }

        public bool TryConsumeAura(float consumeAmount)
        {
            if (currentEnergy >= consumeAmount)
            {
                currentEnergy -= consumeAmount;
                return true;
            }
            return false;
        }

        public bool Active()
        {
            if (currentEnergy > 0) return true;
            return false;
        }

        public virtual IEnumerable<Gizmo> GetGizmos()
        {
            yield break;
        }

        public virtual Color GetColor()
        {
            return pawn.story.hairColor;
        }

        public virtual string GetLabelColor()
        {
            if (GetColor().grayscale > 0.7f) return "<color=#000000>";
            return "<color=#FFFFFF>";
        }

        public void Draw()
        {
            if (pawn.Drafted && Active())
            {
                float num = Mathf.Lerp(1.2f, 1.55f, currentEnergy);
                Vector3 vector = pawn.Drawer.DrawPos;
                vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                int num2 = Find.TickManager.TicksGame - lastAbsorbDamageTick;
                if (num2 < 8)
                {
                    float num3 = (float)(8 - num2) / 8f * 0.05f;
                    vector += impactAngleVect * num3;
                    num -= num3;
                }
                float angle = (float)Rand.Range(0, 360);
                Vector3 s = new Vector3(num, 1f, num);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent, GetColor()), 0);
            }
        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look<float>(ref maxEnergy, "maxEnergy", 1, false);
            Scribe_Values.Look<float>(ref currentEnergy, "currentEnergy", 0, false);
            Scribe_Values.Look<int>(ref lastAbsorbDamageTick, "lastAbsorbDamageTick", -9999, false);
            Scribe_References.Look<Pawn>(ref pawn, "auraOwner", false);
        }

        public float maxEnergy;
        public float currentEnergy;
        public Vector3 impactAngleVect;
        public int lastAbsorbDamageTick = -9999;
        public Pawn pawn;
    }
}
