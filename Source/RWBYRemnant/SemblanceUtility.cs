using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RWBYRemnant
{
    public static class SemblanceUtility
    {
        public static readonly List<string> unlockSemblanceReasons = new List<string>
        {
            "BondedAnimalDied",
            "WitnessedDeathFamily",
            "GotMarried",
            "Catharsis"
        };

        public static readonly List<TraitDef> semblanceList = new List<TraitDef>
        {
            RWBYDefOf.Semblance_Ruby,
            RWBYDefOf.Semblance_Yang,
            RWBYDefOf.Semblance_Weiss,
            RWBYDefOf.Semblance_Blake,
            RWBYDefOf.Semblance_Nora,
            RWBYDefOf.Semblance_Jaune,
            RWBYDefOf.Semblance_Pyrrha,
            RWBYDefOf.Semblance_Ren,
            RWBYDefOf.Semblance_Qrow,
            RWBYDefOf.Semblance_Raven,
            RWBYDefOf.Semblance_Cinder,
            RWBYDefOf.Semblance_Hazel,
            RWBYDefOf.Semblance_Velvet,
            RWBYDefOf.Semblance_Adam
        };

        public static readonly List<string> noraDmgAbsorbDefs = new List<string>
        {
            "TM_Lightning",
            "TM_LightningCloud",
            "SW_ShockBullet_Damage",
            "SW_ShockBullet_Damage_Penetrating",
            "Rimlaser_TeslaGun",
            "PJ_FLightning"
        };

        public static readonly List<HediffDef> injectedDustCrystalHediffs = new List<HediffDef>
        {
            RWBYDefOf.RWBY_InjectedFireCrystal,
            RWBYDefOf.RWBY_InjectedIceCrystal,
            RWBYDefOf.RWBY_InjectedLightningCrystal,
            RWBYDefOf.RWBY_InjectedGravityCrystal,
            RWBYDefOf.RWBY_InjectedHardLightCrystal
        };

        public static IEnumerable<TraitDef> GetSemblancesForPassion(SkillDef skill)
        {
            if (skill == SkillDefOf.Shooting)
            {
                yield return RWBYDefOf.Semblance_Ruby;
                yield return RWBYDefOf.Semblance_Blake;
                yield return RWBYDefOf.Semblance_Cinder;
            }
            if (skill == SkillDefOf.Melee)
            {
                yield return RWBYDefOf.Semblance_Yang;
                yield return RWBYDefOf.Semblance_Hazel;
                yield return RWBYDefOf.Semblance_Qrow;
                yield return RWBYDefOf.Semblance_Adam;
            }
            if (skill == SkillDefOf.Construction)
            {
                yield return RWBYDefOf.Semblance_Raven;
                yield return RWBYDefOf.Semblance_Velvet;
                yield return RWBYDefOf.Semblance_Pyrrha;
            }
            if (skill == SkillDefOf.Mining)
            {
                yield return RWBYDefOf.Semblance_Hazel;
                yield return RWBYDefOf.Semblance_Qrow;
                yield return RWBYDefOf.Semblance_Jaune;
            }
            if (skill == SkillDefOf.Cooking)
            {
                yield return RWBYDefOf.Semblance_Ren;
                yield return RWBYDefOf.Semblance_Qrow;
                yield return RWBYDefOf.Semblance_Nora;
            }
            if (skill == SkillDefOf.Plants)
            {
                yield return RWBYDefOf.Semblance_Yang;
                yield return RWBYDefOf.Semblance_Velvet;
                yield return RWBYDefOf.Semblance_Raven;
                yield return RWBYDefOf.Semblance_Jaune;
            }
            if (skill == SkillDefOf.Animals)
            {
                yield return RWBYDefOf.Semblance_Weiss;
                yield return RWBYDefOf.Semblance_Blake;
                yield return RWBYDefOf.Semblance_Yang;
                yield return RWBYDefOf.Semblance_Adam;
            }
            if (skill == SkillDefOf.Crafting)
            {
                yield return RWBYDefOf.Semblance_Ren;
                yield return RWBYDefOf.Semblance_Pyrrha;
                yield return RWBYDefOf.Semblance_Cinder;
                yield return RWBYDefOf.Semblance_Adam;
            }
            if (skill == SkillDefOf.Artistic)
            {
                yield return RWBYDefOf.Semblance_Ruby;
                yield return RWBYDefOf.Semblance_Nora;
                yield return RWBYDefOf.Semblance_Blake;
            }
            if (skill == SkillDefOf.Medicine)
            {
                yield return RWBYDefOf.Semblance_Raven;
                yield return RWBYDefOf.Semblance_Jaune;
                yield return RWBYDefOf.Semblance_Pyrrha;
                yield return RWBYDefOf.Semblance_Hazel;
            }
            if (skill == SkillDefOf.Social)
            {
                yield return RWBYDefOf.Semblance_Ruby;
                yield return RWBYDefOf.Semblance_Weiss;
                yield return RWBYDefOf.Semblance_Velvet;
                yield return RWBYDefOf.Semblance_Nora;
            }
            if (skill == SkillDefOf.Intellectual)
            {
                yield return RWBYDefOf.Semblance_Weiss;
                yield return RWBYDefOf.Semblance_Ren;
                yield return RWBYDefOf.Semblance_Cinder;
            }
            yield break;
        }

        public static bool UnlockSemblance(Pawn pawn, TraitDef semblance, string textKey, string labelKey = "LetterLabelUnlockSemblanceGeneral")
        {
            if (pawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_AuraStolen)) return false;
            if (!pawn.story.traits.HasTrait(RWBYDefOf.RWBY_Aura)) return false;
            if (pawn.WorkTagIsDisabled(semblance.requiredWorkTags)) return false;
            pawn.story.traits.allTraits.RemoveAll(t => t.def.Equals(RWBYDefOf.RWBY_Aura));
            pawn.story.traits.GainTrait(new Trait(semblance));
            pawn.TryGetComp<CompAbilityUserAura>().Initialize();

            string label = labelKey.Translate().Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN").CapitalizeFirst();
            string text = textKey.Translate().Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN").CapitalizeFirst();
            if (pawn.IsColonistPlayerControlled) Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.PositiveEvent, pawn);
            return true;
        }

        public static bool UnlockAura(Pawn pawn, string textKey, string labelKey = "LetterLabelUnlockAura")
        {
            if (pawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_AuraStolen)) return false;
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().auraUnlockable) return false;
            if (pawn.story.traits.HasTrait(RWBYDefOf.RWBY_Aura)) return false;
            if (semblanceList.Any(s => pawn.story.traits.HasTrait(s))) return false;
            pawn.story.traits.GainTrait(new Trait(RWBYDefOf.RWBY_Aura));
            pawn.TryGetComp<CompAbilityUserAura>().Initialize();

            string label = labelKey.Translate().Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN").CapitalizeFirst();
            string text = textKey.Translate().Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN").CapitalizeFirst();
            if (pawn.IsColonistPlayerControlled) Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.PositiveEvent, pawn);
            return true;
        }

        public static readonly List<PawnKindDef> summonedPawnKindDefs = new List<PawnKindDef>
        {
            RWBYDefOf.Grimm_Boarbatusk_Summoned,
            RWBYDefOf.Grimm_ArmaGigas_Summoned,
            RWBYDefOf.Grimm_ArmaGigasSword_Summoned
        };

        public static bool PyrrhaMagnetismCanAffect(Thing thing)
        {
            if (thing == null) return false;
            if (thing is ThingWithComps thingWithComps && (thingWithComps.Smeltable || thingWithComps.def.IsMetal)) return true;
            if (thing is Pawn pawn && (pawn.RaceProps.IsMechanoid || pawn.RaceProps.FleshType.defName == "ChJDroid" || pawn.RaceProps.FleshType.defName == "Android" || pawn.RaceProps.FleshType.defName == "MechanisedInfantry")) return true;
            
            return false;
        }
    }
}
