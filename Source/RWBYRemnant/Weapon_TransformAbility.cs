using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using Verse;
using Verse.Sound;
using System.Linq;

namespace RWBYRemnant
{
    public class WeaponTransformComp : CompProperties
    {
        public string TransformLabel = "";
        public string TransformDesc;
        public string TransformIcon;
        public SoundDef transformSound;
        public string transformInto;
        public ThingDef usesAmmunition = null;

        public WeaponTransformComp()
        {
            compClass = typeof(Weapon_TransformAbility);
        }
    }

    public class Weapon_TransformAbility : CompUseEffect
    {
        public WeaponTransformComp Props => (WeaponTransformComp)props;
        
        public CompEquippable GetEquippable => parent.GetComp<CompEquippable>();

        public Pawn GetPawn => GetEquippable.verbTracker.PrimaryVerb.CasterPawn;

        public string DestroyDefName => parent.def.defName;

        public void Transform()
        {
            if (GetPawn.equipment.Primary.TryGetComp<LightCopyDestroyAbility>() != null)
            { // destroy light copy
                GetPawn.equipment.Primary.Destroy();
            }                
                
            CompQuality compQualityOld = parent.TryGetComp<CompQuality>();
            int hitPoints = parent.HitPoints;
            ThingWithComps weaponToCreate = new ThingWithComps();

            switch (Props.transformInto)
            {
                case "RWBY_Crocea_Mors_Twohander": //special case from 2 into 1
                    IEnumerator<ThingWithComps> tmpEnumerator = GetPawn.equipment.AllEquipmentListForReading.GetEnumerator();
                    ThingWithComps itemToDestroy = null;
                    while (tmpEnumerator.MoveNext())
                    {
                        ThingWithComps current = tmpEnumerator.Current;
                        if (current.def.defName == "RWBY_Crocea_Mors_Shield")
                        {
                            itemToDestroy = current;
                        }
                    }
                    if (itemToDestroy == null)
                    {                            
                        return;
                    }
                    itemToDestroy.Destroy();
                    weaponToCreate = (ThingWithComps)ThingMaker.MakeThing(ThingDef.Named(Props.transformInto), null);
                    break;
                case "RWBY_Crocea_Mors_Sword": //special case from 1 into 2                        
                    weaponToCreate = (ThingWithComps)ThingMaker.MakeThing(ThingDef.Named(Props.transformInto), null);
                    ThingWithComps itemToCreate = (ThingWithComps)ThingMaker.MakeThing(RWBYDefOf.RWBY_Crocea_Mors_Shield, null);
                    itemToCreate.HitPoints = hitPoints;
                    if (compQualityOld != null)
                    {
                        itemToCreate.TryGetComp<CompQuality>().SetQuality(compQualityOld.Quality, ArtGenerationContext.Colony);
                    }
                    GetPawn.equipment.AddEquipment(itemToCreate);
                    break;
                default: //default case if transform into exactly one item
                    weaponToCreate = (ThingWithComps)ThingMaker.MakeThing(ThingDef.Named(Props.transformInto), null);
                    if (!parent.def.equipmentType.Equals(weaponToCreate.def.equipmentType) && GetPawn.equipment.AllEquipmentListForReading.Find(x => x.def.equipmentType.Equals(weaponToCreate.def.equipmentType)) != null)
                    {
                        return;
                    }                        
                    if (weaponToCreate.TryGetComp<Weapon_TakePhotoAbility>() != null) //additional case it´s a camera or box
                    {
                        weaponToCreate.TryGetComp<Weapon_TakePhotoAbility>().ListOfDifferentPhotos = parent.TryGetComp<Weapon_TakePhotoAbility>().ListOfDifferentPhotos;
                    }
                    break;

            }
                
            weaponToCreate.HitPoints = hitPoints;
            if (compQualityOld != null)
            {
                weaponToCreate.TryGetComp<CompQuality>().SetQuality(compQualityOld.Quality, ArtGenerationContext.Colony);
            }
            if (!ConsumeAmmunition()) return;
            weaponToCreate.ThingID = parent.ThingID;
            weaponToCreate.thingIDNumber = parent.thingIDNumber;
            PlaySound(Props.transformSound);
            Pawn tmpPawn = GetPawn;
            parent.Destroy();
            tmpPawn.equipment.AddEquipment(weaponToCreate);
        }

        public bool ConsumeAmmunition()
        {
            Thing thing = GetPawn.inventory.GetDirectlyHeldThings().ToList().Find(s => s.def == Props.usesAmmunition);
            if (Props.usesAmmunition == null)
            {
                return true;
            }
            else if (Props.usesAmmunition != null && thing == null)
            {
                return false;
            }
            else
            {
                thing.stackCount = thing.stackCount - 1;
                if (thing.stackCount == 0) thing.Destroy();
                return true;
            }
        }

        public virtual void PlaySound(SoundDef soundToPlay)
        {
            SoundInfo info = SoundInfo.InMap(new TargetInfo(GetPawn.PositionHeld, GetPawn.MapHeld, false), MaintenanceType.None);
            soundToPlay?.PlayOneShot(info);
        }

        public Texture2D IconTransform
        {
            get
            {
                var resolvedTexture = TexCommand.GatherSpotActive;
                if (!Props.TransformIcon.NullOrEmpty()) resolvedTexture = ContentFinder<Texture2D>.Get(Props.TransformIcon, true);
                return resolvedTexture;
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            bool disabled = false;
            string disabledReason = "";
            string defaultLabel = "TransformLabel".Translate().CapitalizeFirst();
            if (!string.IsNullOrEmpty(Props.TransformLabel)) defaultLabel = Props.TransformLabel;
            if (GetPawn == null || !GetPawn.equipment.AllEquipmentListForReading.Contains(parent))
            {
                disabled = true;
                disabledReason = "DisabledNotEquipped".Translate(parent.def.label);
            }
            if (Props.usesAmmunition != null && GetPawn != null && GetPawn.inventory.GetDirectlyHeldThings().ToList().Find(s => s.def == Props.usesAmmunition) == null)
            {
                disabled = true;
                disabledReason = "DisabledNoDustPowderAmmunition".Translate(Props.usesAmmunition.label).CapitalizeFirst();
            }
            if (parent.def.defName.Contains("Raven_Sword")) //special case, because Raven´s sword needs its scabbard
            {
                if (GetPawn == null || !GetPawn.equipment.AllEquipmentListForReading.Contains(parent) || GetPawn.equipment.AllEquipmentListForReading.Find(x => x.def.defName.Contains("Raven_Scabbard")) == null)
                {
                    disabled = true;
                    disabledReason = "DisabledRavenSword".Translate();
                }
            }

            yield return new Command_Action
            {
                action = Transform,
                defaultLabel = defaultLabel,
                defaultDesc = Props.TransformDesc,
                icon = IconTransform,
                disabled = disabled,
                disabledReason = disabledReason
            };
        }
    }
}
