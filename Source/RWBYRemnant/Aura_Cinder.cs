using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace RWBYRemnant
{
    public class Aura_Cinder : Aura
    {
        public override bool TryAbsorbDamage(DamageInfo dinfo)
        {
            if (currentEnergy == 0f) return false;
            if ((pawn.Drafted || pawn.IsFighting()) && dinfo.Weapon != null && dinfo.Weapon.IsRangedWeapon && Rand.Chance(0.5f) && TryConsumeAura(0.02f))
            {
                switch (Rand.RangeInclusive(1, 4))
                {
                    case 1:
                        RWBYDefOf.Ricochet1.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
                        break;

                    case 2:
                        RWBYDefOf.Ricochet2.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
                        break;

                    case 3:
                        RWBYDefOf.Ricochet3.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
                        break;

                    case 4:
                        RWBYDefOf.Ricochet4.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
                        break;
                }
                return true;
            }
            return base.TryAbsorbDamage(dinfo);
        }

        public override Color GetColor()
        {
            return color;
        }

        public Color color = new Color(0.8f, 0.4f, 0f);
    }
}
