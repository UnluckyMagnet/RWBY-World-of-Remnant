using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace RWBYRemnant
{
    public class Aura_Adam : Aura
    {
        public override void Tick()
        {
            if (pawn.Downed)
            {
                absorbedDamage = 0f;
            }
            if (!pawn.IsFighting() && Find.TickManager.TicksGame % GenTicks.SecondsToTicks(2) == 0)
            {
                absorbedDamage -= 1f;
                if (absorbedDamage < 0f) absorbedDamage = 0f;                
            }
            base.Tick();
        }

        public override bool TryAbsorbDamage(DamageInfo dinfo)
        {
            if (absorbedDamage < 500f && Rand.Chance(0.7f) && (pawn.Drafted || pawn.IsFighting()) && pawn.equipment.Primary != null && pawn.equipment.Primary.def.IsMeleeWeapon)
            {
                absorbedDamage += dinfo.Amount;
                if (absorbedDamage > 500f) absorbedDamage = 500f;
                RWBYDefOf.Draw_Gambol_Shroud_Katana.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
                return true;
            }
            return base.TryAbsorbDamage(dinfo);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            string label = "AdamAbsorbDamageLabel".Translate().CapitalizeFirst();
            yield return new GizmoAdamAbsorbLevel
            {
                label = label,
                labelColor = GetLabelColor(),
                currentAbsorbedDamage = absorbedDamage,
                FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(GetColor())
            };
        }

        public override Color GetColor()
        {
            return new Color(1f, 0f, 0f);
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
    }
}
