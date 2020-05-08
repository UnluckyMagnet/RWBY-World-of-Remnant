using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

namespace RWBYRemnant
{
    public class Aura_Yang : Aura
    {
        public override void Tick()
        {
            if (pawn.Downed)
            {
                absorbedDamage = 0f;
            }
            if (!pawn.IsFighting() && pawn.IsHashIntervalTick(120))
            {
                absorbedDamage -= 1f;
                if (absorbedDamage < 0f) absorbedDamage = 0f;                
            }
            if (absorbedDamage > 75f)
            {
                Hediff returnDamageHediff = new Hediff();
                returnDamageHediff = HediffMaker.MakeHediff(RWBYDefOf.RWBY_YangReturnDamage, pawn);
                pawn.health.AddHediff(returnDamageHediff);
            }
            base.Tick();
        }

        public override bool TryAbsorbDamage(DamageInfo dinfo)
        {
            if (dinfo.Def.defName == "PJ_ForceHealDamage") return base.TryAbsorbDamage(dinfo);
            absorbedDamage += (dinfo.Amount * 2f);
            if (absorbedDamage > 100f) absorbedDamage = 100f;
            return base.TryAbsorbDamage(dinfo);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            string label = "";
            if (absorbedDamage == 0f)
            {
                label = "AngerLevelNotAngryLabel".Translate().CapitalizeFirst();
            }
            else if (absorbedDamage <= 25f)
            {
                label = "AngerLevelAnnoyedLabel".Translate().CapitalizeFirst();
            }
            else if (absorbedDamage <= 50f)
            {
                label = "AngerLevelAngryLabel".Translate().CapitalizeFirst();
            }
            else if (absorbedDamage <= 75f)
            {
                label = "AngerLevelRagingLabel".Translate().CapitalizeFirst();
            }
            else
            {
                label = "AngerLevelLostHairLabel".Translate().CapitalizeFirst();
            }
            yield return new GizmoYangAngerLevel
            {
                label = label,
                labelColor = GetLabelColor(),
                aura = this,
                currentAbsorbedDamage = absorbedDamage,
                FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(GetColor())
            };
        }

        public override Color GetColor()
        {
            return color;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look<float>(ref maxEnergy, "maxEnergy", 1, false);
            Scribe_Values.Look<float>(ref currentEnergy, "currentEnergy", 0, false);
            Scribe_Values.Look<float>(ref absorbedDamage, "absorbedDamage", 0, false);
            Scribe_Values.Look<int>(ref lastAbsorbDamageTick, "lastAbsorbDamageTick", -9999, false);
            Scribe_References.Look<Pawn>(ref pawn, "auraOwner", false);
        }

        public float absorbedDamage = 0f;
        public Color color = new Color(0.8f, 0.8f, 0f);
    }
}
