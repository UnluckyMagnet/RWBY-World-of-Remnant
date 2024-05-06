using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RWBYRemnant
{
    public class CompWeaponProjectile : ThingComp
    {
        public CompProperties_WeaponProjectile Props => (CompProperties_WeaponProjectile)props;

        public Verb_ShootWeaponAbility verb_ShootWeaponAbility = null;

        public virtual void PlaySound(SoundDef soundToPlay)
        {
            SoundInfo info = SoundInfo.InMap(new TargetInfo(GetPawn().PositionHeld, GetPawn().MapHeld, false), MaintenanceType.None);
            soundToPlay?.PlayOneShot(info);
        }

        public Pawn GetPawn()
        {
            if (parent.holdingOwner != null && parent.holdingOwner.Owner != null && parent.holdingOwner.Owner.ParentHolder != null && parent.holdingOwner.Owner.ParentHolder is Pawn pawn) return pawn;
            return null;
        }

        public Texture2D IconAbility
        {
            get
            {
                var resolvedTexture = TexCommand.GatherSpotActive;
                if (!Props.AbilityIcon.NullOrEmpty()) resolvedTexture = ContentFinder<Texture2D>.Get(Props.AbilityIcon, true);
                return resolvedTexture;
            }
        }

        public void CreateVerb()
        {
            verb_ShootWeaponAbility = new Verb_ShootWeaponAbility
            {
                verbTracker = new VerbTracker(GetPawn()),
                caster = GetPawn(),
                verbProps = new VerbProperties
                {
                    accuracyTouch = 1f,
                    accuracyShort = 1f,
                    accuracyMedium = 1f,
                    accuracyLong = 1f,
                    verbClass = typeof(Verb_ShootWeaponAbility),
                    hasStandardCommand = true,
                    defaultProjectile = Props.AbilityProjectile,
                    warmupTime = Props.WarmupTime,
                    range = Props.Range,
                    soundCast = Props.AbilitySound,
                    burstShotCount = Props.BurstShotCount,
                    ticksBetweenBurstShots = Props.TicksBetweenBurstShots,
                    muzzleFlashScale = 0,
                },
                cannotMiss = Props.cannotMiss,
                ammunition = Props.usesAmmunition
            };
            verb_ShootWeaponAbility.verbProps.targetParams = TargetingParameters.ForAttackAny();
            if (Props.targetingParameters != null) verb_ShootWeaponAbility.verbProps.targetParams = Props.targetingParameters;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (verb_ShootWeaponAbility == null || GetPawn() != verb_ShootWeaponAbility.caster) CreateVerb();

            bool disabled = false;
            string disabledReason = "";

            if (GetPawn() == null)
            {
                disabled = true;
                disabledReason = "DisabledNotEquipped".Translate(parent.def.label);
            }
            else if (!GetPawn().equipment.AllEquipmentListForReading.Contains(parent) && !GetPawn().inventory.innerContainer.Contains(parent))
            {
                disabled = true;
                disabledReason = "DisabledNotEquipped".Translate(parent.def.label);
            }
            else if (Props.usesAmmunition != null && GetPawn().inventory.GetDirectlyHeldThings().ToList().Find(s => s.def == Props.usesAmmunition) == null)
            {
                disabled = true;
                if (GetPawn().story.traits.HasTrait(RWBYDefOf.Semblance_Velvet) && GetPawn().equipment.Primary != null && GetPawn().equipment.Primary.TryGetComp<CompLightCopy>() != null) disabled = false;
                disabledReason = "DisabledNoAmmunition".Translate(Props.usesAmmunition.label).CapitalizeFirst();
            }

            TargetingParameters targetingParameters = TargetingParameters.ForAttackAny();
            if (Props.targetingParameters != null) targetingParameters = Props.targetingParameters;

            yield return new Command_TargetWithRadius
            {
                defaultLabel = Props.AbilityLabel,
                defaultDesc = Props.AbilityDesc,
                icon = IconAbility,
                defaultIconColor = Props.AbilityIconColor,
                disabled = disabled,
                disabledReason = disabledReason,
                verb = verb_ShootWeaponAbility
            };
        }
    }
}