using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RWBYRemnant
{
    public class Aura_Pyrrha : Aura
    {
        public override bool TryAbsorbDamage(DamageInfo dinfo)
        {
            if (currentEnergy > 0 && dinfo.Weapon != null && dinfo.Weapon.IsMeleeWeapon && dinfo.Weapon.smeltable && Rand.Chance(0.3f))
            {
                SoundDefOf.Pawn_Melee_Punch_Miss.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
                return true;
            }
            return base.TryAbsorbDamage(dinfo);
        }

        public override Color GetColor()
        {
            return color;
        }

        public Color color = new Color(1.0f, 0f, 0f);
    }
}
