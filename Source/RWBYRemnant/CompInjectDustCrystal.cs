using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace RWBYRemnant
{
    public class CompInjectDustCrystal : CompUseEffect
    {
        public CompProperties_InjectDustCrystal Props => (CompProperties_InjectDustCrystal)props;

        public Pawn GetPawn()
        {
            if (parent.holdingOwner != null && parent.holdingOwner.Owner != null && parent.holdingOwner.Owner.ParentHolder != null && parent.holdingOwner.Owner.ParentHolder.GetType().Equals(typeof(Pawn))) return (Pawn)parent.holdingOwner.Owner.ParentHolder;
            return null;
        }

        public void DoEffect()
        {
            Pawn pawn = GetPawn();
            Hediff hediffIgnorePain = new Hediff();
            if (parent.def == RWBYDefOf.FireDustCrystal)
            {
                hediffIgnorePain = HediffMaker.MakeHediff(RWBYDefOf.RWBY_InjectedFireCrystal, pawn);
            }
            else if (parent.def == RWBYDefOf.IceDustCrystal)
            {
                hediffIgnorePain = HediffMaker.MakeHediff(RWBYDefOf.RWBY_InjectedIceCrystal, pawn);
            }
            else if (parent.def == RWBYDefOf.LightningDustCrystal)
            {
                hediffIgnorePain = HediffMaker.MakeHediff(RWBYDefOf.RWBY_InjectedLightningCrystal, pawn);
            }
            else if (parent.def == RWBYDefOf.GravityDustCrystal)
            {
                hediffIgnorePain = HediffMaker.MakeHediff(RWBYDefOf.RWBY_InjectedGravityCrystal, pawn);
            }
            else if (parent.def == RWBYDefOf.HardLightDustCrystal)
            {
                hediffIgnorePain = HediffMaker.MakeHediff(RWBYDefOf.RWBY_InjectedHardLightCrystal, pawn);
            }
            else
            {
                return;
            }
            RWBYDefOf.AuraFlicker.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
            parent.stackCount -= 1;
            if (parent.stackCount == 0) parent.Destroy();
            pawn.health.AddHediff(hediffIgnorePain);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (GetPawn() == null) yield break;
            if (!GetPawn().IsColonist) yield break;
            if (GetPawn().TryGetComp<CompAbilityUserAura>() == null || !GetPawn().TryGetComp<CompAbilityUserAura>().IsInitialized) yield break;

            yield return new Command_Action
            {
                defaultLabel = "InjectDustLabel".Translate(parent.def.label),
                defaultDesc = "InjectDustDescription".Translate(parent.def.label),
                icon = parent.def.uiIcon,
                defaultIconColor = parent.def.uiIconColor,
                action = DoEffect
            };
        }
    }
}
