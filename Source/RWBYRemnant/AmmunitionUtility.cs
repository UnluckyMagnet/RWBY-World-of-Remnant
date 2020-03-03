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
            if (pawn.equipment.Primary != null && pawn.equipment.Primary.TryGetComp<CompWeaponTransform>() != null)
            {
                foreach (CompWeaponTransform thingComp in pawn.equipment.Primary.AllComps.FindAll(c => c.GetType().Equals(typeof(CompWeaponTransform))))
                {
                    if (thingComp.Props.usesAmmunition != null) yield return thingComp.Props.usesAmmunition;
                }
            }
            if (pawn.equipment.Primary != null && pawn.equipment.Primary.TryGetComp<CompWeaponProjectile>() != null)
            {
                foreach (CompWeaponProjectile thingComp in pawn.equipment.Primary.AllComps.FindAll(c => c.GetType().Equals(typeof(CompWeaponProjectile))))
                {
                    if (thingComp.Props.usesAmmunition != null) yield return thingComp.Props.usesAmmunition;
                }
            }
            ThingWithComps thingWithComps1 = pawn.equipment.AllEquipmentListForReading.Find(t => t.def == RWBYDefOf.RWBY_Anesidora_Camera);
            if (thingWithComps1 != null)
            {
                yield return thingWithComps1.TryGetComp<CompTakePhoto>().Props.usesAmmunition;
            }
            ThingWithComps thingWithComps2 = pawn.equipment.AllEquipmentListForReading.Find(t => t.def == RWBYDefOf.RWBY_Anesidora_Box);
            if (thingWithComps2 != null)
            {
                yield return thingWithComps2.TryGetComp<CompTakePhoto>().Props.usesAmmunition;
            }
            yield break;
        }
    }
}
