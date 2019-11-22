using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RWBYRemnant
{
    public static class AmmunitionUtility
    {
        public static IEnumerable<ThingDef> NeedsAmmunition(Pawn pawn)
        {
            foreach (AbilitySemblance abilitySemblance in pawn.TryGetComp<CompAbilityUserAura>().AbilityData.AllPowers)
            {
                if (abilitySemblance.SemblanceDef.usesAmmunition != null) yield return abilitySemblance.SemblanceDef.usesAmmunition;
            }
            if (pawn.story != null && pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Hazel))
            {
                yield return RWBYDefOf.FireDustCrystal;
                yield return RWBYDefOf.IceDustCrystal;
                yield return RWBYDefOf.LightningDustCrystal;
                yield return RWBYDefOf.GravityDustCrystal;
                yield return RWBYDefOf.HardLightDustCrystal;
            }
            if (pawn.equipment.Primary != null && pawn.equipment.Primary.TryGetComp<Weapon_TransformAbility>() != null)
            {
                foreach (Weapon_TransformAbility thingComp in pawn.equipment.Primary.AllComps.FindAll(c => c.GetType().Equals(typeof(Weapon_TransformAbility))))
                {
                    if (thingComp.Props.usesAmmunition != null) yield return thingComp.Props.usesAmmunition;
                }
            }
            if (pawn.equipment.Primary != null && pawn.equipment.Primary.TryGetComp<Weapon_ProjectileAbility>() != null)
            {
                foreach (Weapon_ProjectileAbility thingComp in pawn.equipment.Primary.AllComps.FindAll(c => c.GetType().Equals(typeof(Weapon_ProjectileAbility))))
                {
                    if (thingComp.Props.usesAmmunition != null) yield return thingComp.Props.usesAmmunition;
                }
            }
            ThingWithComps thingWithComps1 = pawn.equipment.AllEquipmentListForReading.Find(t => t.def == RWBYDefOf.RWBY_Velvet_Camera);
            if (thingWithComps1 != null)
            {
                yield return thingWithComps1.TryGetComp<Weapon_TakePhotoAbility>().Props.usesAmmunition;
            }
            ThingWithComps thingWithComps2 = pawn.equipment.AllEquipmentListForReading.Find(t => t.def == RWBYDefOf.RWBY_Velvet_Camera_Box);
            if (thingWithComps2 != null)
            {
                yield return thingWithComps2.TryGetComp<Weapon_TakePhotoAbility>().Props.usesAmmunition;
            }
            yield break;
        }
    }
}
