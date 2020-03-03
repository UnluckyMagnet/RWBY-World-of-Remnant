using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace RWBYRemnant
{
    public class CompWeaponDrinkCoffee : CompUseEffect
    {
        public CompProperties_WeaponDrinkCoffee Props => (CompProperties_WeaponDrinkCoffee)props;

        public CompEquippable GetEquippable => parent.GetComp<CompEquippable>();

        public Pawn GetPawn => GetEquippable.verbTracker.PrimaryVerb.CasterPawn;

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

        public void DrinkCoffee()
        {
            PlaySound(Props.AbilitySound);
            Hediff hediffCaffeine = new Hediff();
            hediffCaffeine = HediffMaker.MakeHediff(RWBYDefOf.RWBY_Thermos_Caffeine, GetPawn);
            GetPawn.health.AddHediff(hediffCaffeine);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            bool disabled = false;
            string disabledReason = "";
            if (GetPawn == null || !GetPawn.equipment.AllEquipmentListForReading.Contains(parent))
            {                
                disabled = true;
                disabledReason = "DisabledNotEquipped".Translate(parent.def.label);
            }
            yield return new Command_Action
            {
                defaultLabel = Props.AbilityLabel,
                defaultDesc = Props.AbilityDesc,
                icon = IconAbility,
                action = DrinkCoffee,
                disabled = disabled,
                disabledReason = disabledReason
            };
        }
    }
}
