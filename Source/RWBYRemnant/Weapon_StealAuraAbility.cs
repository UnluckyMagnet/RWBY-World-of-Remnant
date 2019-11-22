using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RWBYRemnant
{
    public class Weapon_StealAuraComp : CompProperties
    {
        public string AbilityLabelSteal;
        public string AbilityDescSteal;
        public string AbilityIconSteal;
        public string AbilityLabelConsume;
        public string AbilityDescConsume;
        public string AbilityIconConsume;

        public Weapon_StealAuraComp()
        {
            compClass = typeof(Weapon_StealAuraAbility);
        }
    }

    public class Weapon_StealAuraAbility : CompUseEffect
    {
        public Weapon_StealAuraComp Props => (Weapon_StealAuraComp)props;

        public CompEquippable GetEquippable => parent.GetComp<CompEquippable>();

        public Pawn GetPawn => GetEquippable.verbTracker.PrimaryVerb.CasterPawn;

        public TraitDef stolenTraitDef = null;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look<TraitDef>(ref stolenTraitDef, "stolenTraitDef");
        }

        public Texture2D IconAbilitySteal
        {
            get
            {
                var resolvedTexture = TexCommand.GatherSpotActive;
                if (!Props.AbilityIconSteal.NullOrEmpty()) resolvedTexture = ContentFinder<Texture2D>.Get(Props.AbilityIconSteal, true);
                return resolvedTexture;
            }
        }

        public Texture2D IconAbilityConsume
        {
            get
            {
                var resolvedTexture = TexCommand.GatherSpotActive;
                if (!Props.AbilityIconConsume.NullOrEmpty()) resolvedTexture = ContentFinder<Texture2D>.Get(Props.AbilityIconConsume, true);
                return resolvedTexture;
            }
        }

        public void DoEffectOn(Thing target)
        {
            if (target is Pawn targetPawn && targetPawn.RaceProps.Humanlike && targetPawn.story.traits.allTraits.Any(t => t.def == RWBYDefOf.RWBY_Aura || SemblanceUtility.semblanceList.Contains(t.def)))
            {
                WorkGiver_StealAura stealAuraWorker = new WorkGiver_StealAura();
                Job job = stealAuraWorker.JobOnThing(GetPawn, targetPawn, parent, true);
                GetPawn.jobs.TryTakeOrderedJob(job);
            }
        }

        public void ConsumeStolenAura()
        {
            Hediff hediff = GetPawn.health.hediffSet.hediffs.Find(h => h. def == RWBYDefOf.RWBY_AuraStolen);
            if (hediff != null) GetPawn.health.RemoveHediff(hediff);
            if (stolenTraitDef != null && GetPawn.TryGetComp<CompAbilityUserAura>() is CompAbilityUserAura compAbilityUserAura)
            {
                if (compAbilityUserAura.IsInitialized && GetPawn.story.traits.allTraits.Any(t => SemblanceUtility.semblanceList.Contains(t.def))) // if pawn has Semblance
                {
                    compAbilityUserAura.aura.maxEnergy += 0.5f;
                    // Aura specific: Yang, Blake, Raven
                    // no active abilities: Nora, Qrow, Velvet
                    if (stolenTraitDef == RWBYDefOf.Semblance_Ruby)
                    {
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Ruby_BurstIntoRosePetals)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Ruby_BurstIntoRosePetals);
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Ruby_CarryPawn)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Ruby_CarryPawn);
                    }
                    else if (stolenTraitDef == RWBYDefOf.Semblance_Weiss)
                    {
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Weiss_TimeDilationGlyph_Summon)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Weiss_TimeDilationGlyph_Summon);
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Weiss_SummonBoar)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Weiss_SummonBoar);
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Weiss_SummonArmaGigas)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Weiss_SummonArmaGigas);
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Weiss_SummonArmaGigasSword)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Weiss_SummonArmaGigasSword);
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Weiss_SummonLancer)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Weiss_SummonLancer);
                    }
                    else if (stolenTraitDef == RWBYDefOf.Semblance_Jaune)
                    {
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Jaune_AmplifyAura)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Jaune_AmplifyAura);
                    }
                    else if (stolenTraitDef == RWBYDefOf.Semblance_Pyrrha)
                    {
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Pyrrha_UnlockAura)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Pyrrha_UnlockAura);
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Pyrrha_UseMagnetism)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Pyrrha_UseMagnetism);
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Pyrrha_MagneticPulse)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Pyrrha_MagneticPulse);
                    }
                    else if (stolenTraitDef == RWBYDefOf.Semblance_Ren)
                    {
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Ren_MaskEmotions)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Ren_MaskEmotions);
                    }
                    else if (stolenTraitDef == RWBYDefOf.Semblance_Cinder)
                    {
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Cinder_ShootFireCrystal)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Cinder_ShootFireCrystal);
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Cinder_SummonExplosives)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Cinder_SummonExplosives);
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Cinder_CreateBlades)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Cinder_CreateBlades);
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Cinder_CreateBow)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Cinder_CreateBow);
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Cinder_CreateScimitar)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Cinder_CreateScimitar);
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Cinder_CreateSpear)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Cinder_CreateSpear);
                    }
                    else if (stolenTraitDef == RWBYDefOf.Semblance_Hazel)
                    {
                        if (!compAbilityUserAura.AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Hazel_IgnorePain)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Hazel_IgnorePain);
                    }
                    RWBYDefOf.AuraFlicker.PlayOneShot(new TargetInfo(GetPawn.Position, GetPawn.Map, false));
                }
                else
                {
                    GetPawn.story.traits.allTraits.RemoveAll(t => t.def.Equals(RWBYDefOf.RWBY_Aura));
                    GetPawn.story.traits.GainTrait(new Trait(stolenTraitDef));
                    GetPawn.TryGetComp<CompAbilityUserAura>().Initialize();
                    RWBYDefOf.AuraBreak.PlayOneShot(new TargetInfo(GetPawn.Position, GetPawn.Map, false));
                }
                parent.Destroy();
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            bool disabled = false;
            string disabledReason = "";

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
            TargetingParameters targetingParameters = TargetingParameters.ForAttackAny();

            if (stolenTraitDef == null)
            {
                yield return new Command_Target
                {
                    defaultLabel = Props.AbilityLabelSteal,
                    defaultDesc = Props.AbilityDescSteal,
                    icon = IconAbilitySteal,
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
                    disabledReason = disabledReason
                };
            }
            else
            {
                yield return new Command_Action
                {
                    defaultLabel = Props.AbilityLabelConsume,
                    defaultDesc = Props.AbilityDescConsume,
                    icon = IconAbilityConsume,
                    action = ConsumeStolenAura,
                    disabled = disabled,
                    disabledReason = disabledReason
                };
            }            
        }
    }
}
