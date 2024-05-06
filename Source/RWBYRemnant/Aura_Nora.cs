using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace RWBYRemnant
{
    public class Aura_Nora : Aura
    {
        public override bool TryAbsorbDamage(DamageInfo dinfo)
        {
            if (nextDamageIsLightning || dinfo.Def == DamageDefOf.EMP || dinfo.Def == RWBYDefOf.Bomb_Lightning || dinfo.Def == RWBYDefOf.RWBY_Lightning_Slash || dinfo.Def == RWBYDefOf.RWBY_Lightning_Blunt || dinfo.Def == RWBYDefOf.RWBY_Lightning_Bullet || SemblanceUtility.noraDmgAbsorbDefs.Contains(dinfo.Def.defName) || (dinfo.Weapon != null && SemblanceUtility.noraDmgAbsorbDefs.Contains(dinfo.Weapon.defName)))
            {
                nextDamageIsLightning = false;
                if (dinfo.Amount < 10f)
                {
                    currentEnergy += 0.1f;
                }
                else
                {
                    currentEnergy = currentEnergy + (dinfo.Amount / 100f);
                }
                if (currentEnergy > maxEnergy) currentEnergy = maxEnergy;
                Hediff hediffCharged = new Hediff();
                hediffCharged = HediffMaker.MakeHediff(RWBYDefOf.RWBY_LightningBuff, pawn);
                pawn.health.AddHediff(hediffCharged);
                return true;
            }

            return base.TryAbsorbDamage(dinfo);
        }

        public void Notify_NextDamageIsLightning()
        {
            nextDamageIsLightning = true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref nextDamageIsLightning, "nextDamageIsLightning", false, false);
        }

        public override Color GetColor()
        {
            return color;
        }

        private bool nextDamageIsLightning = false;
        public Color color = new Color(1.0f, 0.6f, 1.0f);
    }
}
