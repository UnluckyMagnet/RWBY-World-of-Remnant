using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace RWBYRemnant
{
    public class Aura_Blake : Aura
    {
        public override bool TryAbsorbDamage(DamageInfo dinfo)
        {
            if (dinfo.Def.defName == "PJ_ForceHealDamage") return base.TryAbsorbDamage(dinfo);
            if (dinfo.Instigator != null && dinfo.Instigator.def == RWBYDefOf.Blake_ShadowClone_Fire) return true;
            if ((pawn.Drafted || pawn.IsFighting()) && Rand.Chance(0.5f))
            {
                Building edifice = pawn.Position.GetEdifice(pawn.Map);
                if ((edifice == null || edifice.def.Fillage == FillCategory.None) && TryConsumeAura(0.02f))
                {
                    UseShadowClone(dinfo);
                    return true;
                }
            }
            return base.TryAbsorbDamage(dinfo);
        }

        public void UseShadowClone(DamageInfo dinfo)
        {
            ThingDef cloneToUse = RWBYDefOf.Blake_ShadowClone; // default
            float cloneDuration = 2f;
            Color color = new Color();
            if (remainingUses > 0  && elementalShadowCloneToUse != null)
            {
                remainingUses -= 1;
                cloneToUse = elementalShadowCloneToUse;
                cloneDuration = elementalShadowCloneDuration;
                color = elementalShadowCloneColor;
            }
            if (cloneToUse == RWBYDefOf.Blake_ShadowClone_Ice && dinfo.Instigator is Pawn attackerPawn && dinfo.Weapon != null && !dinfo.Weapon.IsRangedWeapon) // add stun(freeze) to melee attacker
            {
                attackerPawn.stances.stunner.StunFor(GenTicks.SecondsToTicks(4.5f), pawn);
            }
            Thing thing = ThingMaker.MakeThing(cloneToUse);
            thing.TryGetComp<CompShadowClone>().SetData(pawn, GenTicks.SecondsToTicks(cloneDuration), color);
            GenSpawn.Spawn(thing, pawn.Position, pawn.Map);
            thing.TryGetComp<CompShadowClone>().MovePawnOut();
            if (cloneToUse == RWBYDefOf.Blake_ShadowClone_Fire) thing.TryGetComp<CompExplosiveSilent>().StartWick();
        }

        public void SetClone(ThingDef thingDef, float cloneDuration, Color color)
        {
            elementalShadowCloneToUse = thingDef;
            elementalShadowCloneDuration = cloneDuration;
            elementalShadowCloneColor = color;
            remainingUses = 3;
        }

        public override Color GetColor()
        {
            return color;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<ThingDef>(ref elementalShadowCloneToUse, "elementalShadowCloneToUse");
            Scribe_Values.Look<float>(ref elementalShadowCloneDuration, "elementalShadowCloneDuration", 2f, false);
            Scribe_Values.Look<Color>(ref elementalShadowCloneColor, "elementalShadowCloneColor", new Color(), false);
            Scribe_Values.Look<int>(ref remainingUses, "remainingUses", 0, false);
        }

        public ThingDef elementalShadowCloneToUse;
        public float elementalShadowCloneDuration;
        public Color elementalShadowCloneColor;
        public int remainingUses;
        public Color color = new Color(0.8f, 0.2f, 1.0f);
    }
}
