using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RWBYRemnant
{
    public class WeaponProjectileComp : CompProperties
    {
        public string AbilityLabel;
        public string AbilityDesc;
        public string AbilityIcon;
        public SoundDef AbilitySound;
        public SoundDef WarmupSound = null;
        public ThingDef AbilityProjectile;
        public float WarmupTime = 0;
        public float Range;
        public int BurstShotCount = 1;
        public int TicksBetweenBurstShots = 1;
        public bool cannotMiss = false;
        public TargetingParameters targetingParameters = null;
        public ThingDef usesAmmunition = null;

        public WeaponProjectileComp()
        {
            compClass = typeof(Weapon_ProjectileAbility);
        }
    }

    public class Weapon_ProjectileAbility : CompUseEffect
    {
        public WeaponProjectileComp Props => (WeaponProjectileComp)props;

        public CompEquippable GetEquippable => parent.GetComp<CompEquippable>();

        public Pawn GetPawn => GetEquippable.verbTracker.PrimaryVerb.CasterPawn;

        public Verb_ShootWeaponAbility verb_Shoot = new Verb_ShootWeaponAbility();

        public int removeAtTick = 0;

        public virtual void PlaySound(SoundDef soundToPlay)
        {
            SoundInfo info = SoundInfo.InMap(new TargetInfo(GetPawn.PositionHeld, GetPawn.MapHeld, false), MaintenanceType.None);
            soundToPlay?.PlayOneShot(info);
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

        public void DoEffectOn(Thing target)
        {
            if (target == null && !Props.targetingParameters.canTargetLocations) return;
            IntVec3 intVec3 = UI.MouseMapPosition().ToIntVec3();
            if (Props.WarmupSound != null) PlaySound(Props.WarmupSound);
            float statValue = GetPawn.GetStatValue(StatDefOf.AimingDelayFactor, true);
            int warmupTicks = (Props.WarmupTime * statValue).SecondsToTicks();
            removeAtTick = Find.TickManager.TicksGame + (Props.TicksBetweenBurstShots * Props.BurstShotCount) + warmupTicks + 1;
            verb_Shoot = new Verb_ShootWeaponAbility();
            verb_Shoot.verbTracker = new VerbTracker(GetPawn);
            verb_Shoot.caster = GetPawn;
            VerbProperties verbProperties = new VerbProperties
            {
                accuracyTouch = 1.50f,
                accuracyShort = 1.50f,
                accuracyMedium = 1.50f,
                accuracyLong = 1.50f,
                verbClass = typeof(Verb_Shoot),
                hasStandardCommand = true,
                defaultProjectile = Props.AbilityProjectile,
                warmupTime = Props.WarmupTime,
                range = Props.Range,
                soundCast = Props.AbilitySound,
                burstShotCount = Props.BurstShotCount,
                ticksBetweenBurstShots = Props.TicksBetweenBurstShots,
                muzzleFlashScale = 0,
                forcedMissRadius = 0f,                
            };
            verb_Shoot.verbProps = verbProperties;
            verb_Shoot.cannotMiss = Props.cannotMiss;
            verb_Shoot.ammunition = Props.usesAmmunition;

            GetPawn.verbTracker.AllVerbs.Add(verb_Shoot);
            if (target != null)
            {
                if (GetPawn.jobs != null && GetPawn.jobs.curJob != null)
                {
                    GetPawn.jobs.StopAll(false);
                }
                if (GetPawn.pather != null) GetPawn.pather.StopDead();
                GetPawn.ClearAllReservations();
                GetPawn.verbTracker.AllVerbs.Last().TryStartCastOn(target);
            }
            else
            {
                if (GetPawn.jobs != null && GetPawn.jobs.curJob != null)
                {
                    GetPawn.jobs.StopAll(false);
                }
                if (GetPawn.pather != null) GetPawn.pather.StopDead();
                GetPawn.ClearAllReservations();
                GetPawn.verbTracker.AllVerbs.Last().TryStartCastOn(intVec3);
            }
        }

        public void RemoveCustomVerb()
        {
            if (GetPawn != null && GetPawn.verbTracker.AllVerbs.Contains(verb_Shoot))
            {
                if (Find.TickManager.TicksGame > removeAtTick)
                {
                    GetPawn.verbTracker.AllVerbs.Remove(verb_Shoot);
                }
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            RemoveCustomVerb();

            bool disabled = false;
            string disabledReason = "";
            IntVec3 center = parent.Position;

            if (GetPawn == null)
            {
                disabled = true;
                disabledReason = "DisabledNotEquipped".Translate(parent.def.label);
            }
            else if (!GetPawn.equipment.AllEquipmentListForReading.Contains(parent))
            {
                disabled = true;
                disabledReason = "DisabledNotEquipped".Translate(parent.def.label);
            }            
            else if (Props.usesAmmunition != null && GetPawn.inventory.GetDirectlyHeldThings().ToList().Find(s => s.def == Props.usesAmmunition) == null)
            {
                disabled = true;
                if (GetPawn.story.traits.HasTrait(RWBYDefOf.Semblance_Velvet) && GetPawn.equipment.Primary != null && GetPawn.equipment.Primary.TryGetComp<LightCopyDestroyAbility>() != null) disabled = false;
                center = GetPawn.Position;
                disabledReason = "DisabledNoDustPowderAmmunition".Translate(Props.usesAmmunition.label).CapitalizeFirst();
            }
            else if (GetPawn.verbTracker.AllVerbs.Contains(verb_Shoot))
            {
                disabled = true;
                center = GetPawn.Position;
                disabledReason = "DisabledStillShooting".Translate(GetPawn.Name.ToStringShort);
            }
            else
            {
                center = GetPawn.Position;
            }

            TargetingParameters targetingParameters = TargetingParameters.ForAttackAny();
            if (Props.targetingParameters != null) targetingParameters = Props.targetingParameters;

            yield return new Command_TargetWithRadius
            {
                defaultLabel = Props.AbilityLabel,
                defaultDesc = Props.AbilityDesc,
                icon = IconAbility,
                targetingParams = targetingParameters,
                action = delegate (Thing target)
                {
                    IEnumerable<Pawn> enumerable = Find.Selector.SelectedObjects.Where(delegate (object x)
                    {
                        Pawn pawn3 = x as Pawn;
                        return pawn3 != null && pawn3.IsColonistPlayerControlled && pawn3.Drafted;
                    }).Cast<Pawn>();
                    DoEffectOn(target);
                },
                disabled = disabled,
                disabledReason = disabledReason,
                center = center,
                range = Props.Range
            };
        }
    }
}
