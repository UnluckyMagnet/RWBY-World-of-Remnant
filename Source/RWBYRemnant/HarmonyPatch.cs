using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RWBYRemnant
{
    [StaticConstructorOnStartup]
    static class HarmonyPatch
    {
        static HarmonyPatch()
        {
            var harmony = new Harmony("rimworld.carnysenpai.rwbyremnant");

            harmony.Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), "GetGizmos"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("GetGizmos_PostFix")), null); // adds abilities to pawns
            harmony.Patch(AccessTools.Method(typeof(GenDrop), "TryDropSpawn"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("TryDropSpawn_PostFix")), null); // lets light copies disappear on drop
            harmony.Patch(AccessTools.Method(typeof(Verb_MeleeAttackDamage), "DamageInfosToApply"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("DamageInfosToApply_PostFix")), null); // strenghtens certain pawns melee attacks
            harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "PreApplyDamage"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("PreApplyDamage_PostFix")), null); // aura absorb
            harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "NotifyPlayerOfKilled"), new HarmonyMethod(typeof(HarmonyPatch).GetMethod("PreNotifyPlayerOfKilled_PreFix")), null, null); // disables notification if summoned Grimm disappears
            harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "AddHediff", new[] { typeof(Hediff), typeof(BodyPartRecord), typeof(DamageInfo), typeof(DamageWorker.DamageResult) }), new HarmonyMethod(typeof(HarmonyPatch).GetMethod("AddHediff_PreFix")), null, null);  // makes Nora immune to RimTasers Reloaded debuff and charges her
            harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "RenderPawnAt", new[] { typeof(Vector3), typeof(Rot4?), typeof(bool) }), new HarmonyMethod(typeof(HarmonyPatch).GetMethod("RenderPawnAt_PreFix")), null, null); // makes invisible: Ruby while dashing, Apathy while not triggered
            harmony.Patch(AccessTools.Method(typeof(DamageWorker_Flame), "ExplosionAffectCell"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("ExplosionAffectCell_PostFix")), null); // makes fire Dust spawn fire on explosion
            harmony.Patch(AccessTools.Method(typeof(JobDriver_Wait), "CheckForAutoAttack"), new HarmonyMethod(typeof(HarmonyPatch).GetMethod("CheckForAutoAttack_PreFix")), null, null); // fixes summoned Grimm bug of nullpointer if wandering
            harmony.Patch(AccessTools.Method(typeof(WeatherEvent_LightningStrike), "FireEvent"), new HarmonyMethod(typeof(HarmonyPatch).GetMethod("FireEvent_PreFix")), null, null); // changes lightning stike location onto Nora pawns
            harmony.Patch(AccessTools.Method(typeof(Thing), "Ingested"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("Ingested_PostFix")), null); // checks for Pumpkin Pete´s eaten
            harmony.Patch(AccessTools.Method(typeof(Pawn_RecordsTracker), "Increment"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("Increment_PostFix")), null); // unlocks Semblance Shooting Melee Construction Mining Cooking Plants Animals Medicine
            harmony.Patch(AccessTools.Method(typeof(Pawn_JobTracker), "StartJob"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("StartJob_PostFix")), null); // unlocks Semblance Intellectual
            harmony.Patch(AccessTools.Method(typeof(RecordsUtility), "Notify_BillDone"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("Notify_BillDone_PostFix")), null); // unlocks Semblance Crafting Artistic
            harmony.Patch(AccessTools.Method(typeof(Trait), "TipString"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("TipString_PostFix")), null); // adds disabled working tags to Trait descriptions
            harmony.Patch(AccessTools.Method(typeof(GenHostility), "HostileTo", new[] { typeof(Thing), typeof(Thing) }), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("HostileTo_PostFix")), null); // makes Grimm unable to attack pawns affected by Ren or without negative emotions
            harmony.Patch(AccessTools.Method(typeof(IncidentWorker_RaidEnemy), "TryExecuteWorker"), new HarmonyMethod(typeof(HarmonyPatch).GetMethod("TryExecuteWorker_PreFix")), null, null);  // may increases raid size if Semblance Qrow is present
            harmony.Patch(AccessTools.Method(typeof(AttackTargetFinder), "BestAttackTarget"), new HarmonyMethod(typeof(HarmonyPatch).GetMethod("BestAttackTarget_PreFix")), null, null); // makes Grimm not need line of sight
            harmony.Patch(AccessTools.Method(typeof(Pawn_InteractionsTracker), "TryInteractWith"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("TryInteractWith_PostFix")), null); // unlock Semblance Social
            harmony.Patch(AccessTools.Method(typeof(Targeter), "ProcessInputEvents"), new HarmonyMethod(typeof(HarmonyPatch).GetMethod("ProcessInputEvents_Prefix")), null, null); // lets the weapon projectile ability aim properly
            harmony.Patch(AccessTools.Method(typeof(RecordsUtility), "Notify_PawnKilled"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("Notify_PawnKilled_PostFix")), null); // add Weiss summon Grimm ability
            harmony.Patch(AccessTools.Method(typeof(PawnGenerator), "GeneratePawn", new[] { typeof(PawnGenerationRequest) }), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("GeneratePawn_PostFix")), null); // adds silver eyes to a humanoid pawn
        }

        #region "unlock Semblances"

        [HarmonyPostfix]
        public static void Notify_BillDone_PostFix(Pawn billDoer, List<Thing> products) // unlocks Semblance Crafting Artistic
        {
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().semblanceUnlockable) return;
            if (billDoer.GetComp<CompAbilityUserAura>() is CompAbilityUserAura compAbilityUserAura && billDoer.story != null && billDoer.story.traits.HasTrait(RWBYDefOf.RWBY_Aura))
            {
                foreach (var product in products)
                {
                    if (product.def.IsNutritionGivingIngestible && product.def.ingestible.preferability >= FoodPreferability.MealAwful)
                    {
                        // do nothing
                    }
                    else if (product.def.HasComp(typeof(CompArt)) && product.TryGetComp<CompArt>() is CompArt compArt && compArt.Active)
                    {
                        if (Rand.Chance(0.05f)) compAbilityUserAura.TryUnlockSemblanceWith(SkillDefOf.Artistic);
                    }
                    else if (!product.def.HasComp(typeof(CompArt)))
                    {
                        if (Rand.Chance(0.005f)) compAbilityUserAura.TryUnlockSemblanceWith(SkillDefOf.Crafting);
                    }
                }
            }
        }

        [HarmonyPostfix]
        public static void TryInteractWith_PostFix(Pawn ___pawn, Pawn recipient, bool __result) // unlock Semblance Social
        {
            if (!__result) return;
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().semblanceUnlockable) return;
            if (___pawn.GetComp<CompAbilityUserAura>() is CompAbilityUserAura compAbilityUserAura && ___pawn.story != null && ___pawn.story.traits.HasTrait(RWBYDefOf.RWBY_Aura))
            {
                if (Rand.Chance(0.001f)) compAbilityUserAura.TryUnlockSemblanceWith(SkillDefOf.Social);
            }
        }

        [HarmonyPostfix]
        public static void StartJob_PostFix(Pawn_JobTracker __instance, Job newJob, Pawn ___pawn) // unlocks Semblance Intellectual
        {
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().semblanceUnlockable) return;
            if (___pawn.GetComp<CompAbilityUserAura>() is CompAbilityUserAura compAbilityUserAura && ___pawn.story != null && ___pawn.story.traits.HasTrait(RWBYDefOf.RWBY_Aura))
            {
                if (newJob.def == JobDefOf.Research)
                {
                    if (Rand.Chance(0.01f)) compAbilityUserAura.TryUnlockSemblanceWith(SkillDefOf.Intellectual);
                }
            }
        }

        [HarmonyPostfix]
        public static void Increment_PostFix(Pawn_RecordsTracker __instance, RecordDef def) // unlocks Semblance Shooting Melee Construction Mining Cooking Plants Animals Medicine
        {
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().semblanceUnlockable) return;
            if (__instance.pawn.GetComp<CompAbilityUserAura>() is CompAbilityUserAura compAbilityUserAura && __instance.pawn.story != null && __instance.pawn.story.traits.HasTrait(RWBYDefOf.RWBY_Aura))
            {
                if (def == RecordDefOf.ShotsFired)
                {
                    if (Rand.Chance(0.005f)) compAbilityUserAura.TryUnlockSemblanceWith(SkillDefOf.Shooting);
                }
                else if (def == RecordDefOf.DamageTaken)
                {
                    if (Rand.Chance(0.005f)) compAbilityUserAura.TryUnlockSemblanceWith(SkillDefOf.Melee);
                }
                else if (def == RecordDefOf.ThingsConstructed)
                {
                    if (Rand.Chance(0.005f)) compAbilityUserAura.TryUnlockSemblanceWith(SkillDefOf.Construction);
                }
                else if (def == RecordDefOf.CellsMined)
                {
                    if (Rand.Chance(0.005f)) compAbilityUserAura.TryUnlockSemblanceWith(SkillDefOf.Mining);
                }
                else if (def == RecordDefOf.MealsCooked)
                {
                    if (Rand.Chance(0.005f)) compAbilityUserAura.TryUnlockSemblanceWith(SkillDefOf.Cooking);
                }
                else if (def == RecordDefOf.PlantsHarvested)
                {
                    if (Rand.Chance(0.005f)) compAbilityUserAura.TryUnlockSemblanceWith(SkillDefOf.Plants);
                }
                else if (def == RecordDefOf.AnimalsSlaughtered || def == RecordDefOf.AnimalsTamed)
                {
                    if (Rand.Chance(0.01f)) compAbilityUserAura.TryUnlockSemblanceWith(SkillDefOf.Animals);
                }
                else if (def == RecordDefOf.TimesTendedOther)
                {
                    if (Rand.Chance(0.01f)) compAbilityUserAura.TryUnlockSemblanceWith(SkillDefOf.Medicine);
                }
            }
        }

        #endregion

        #region "Grimm"

        [HarmonyPostfix]
        public static void HostileTo_PostFix(ref bool __result, Thing a, Thing b) // makes Grimm unable to attack pawns affected by Ren or without negative emotions
        {
            if (!__result) return;
            if (!(a is Pawn searcherPawn)) return;
            if (!GrimmUtility.IsGrimm(searcherPawn) || searcherPawn.Faction.def != RWBYDefOf.Creatures_of_Grimm) return;
            if (b is Pawn targetPawn && (targetPawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_MaskedEmotions) || targetPawn.Downed))
            {
                __result = false;
                return;
            }
            if (b is Pawn targetPawn2 && targetPawn2.RaceProps.Humanlike)
            {
                List<Thought> outThoughts = new List<Thought>();
                targetPawn2.needs.mood.thoughts.GetAllMoodThoughts(outThoughts);
                if (!outThoughts.Any(o => o.MoodOffset() < 0f)) __result = false;
            }
        }

        [HarmonyPrefix]
        public static void BestAttackTarget_PreFix(IAttackTargetSearcher searcher, ref TargetScanFlags flags) // makes Grimm not need line of sight
        {
            if (!(searcher is Pawn searcherPawn)) return;
            if (searcherPawn.Faction == null) return;
            if (!GrimmUtility.IsGrimm(searcherPawn) || searcherPawn.Faction.def != RWBYDefOf.Creatures_of_Grimm) return;
            flags = TargetScanFlags.None;
        }

        [HarmonyPrefix]
        public static bool CheckForAutoAttack_PreFix(JobDriver_Wait __instance) // fixes summoned Grimm bug of nullpointer if wandering
        {
            if (GrimmUtility.IsGrimm(__instance.pawn))
            {
                if (__instance.pawn.Downed)
                {
                    return false;
                }
                if (__instance.pawn.stances.FullBodyBusy)
                {
                    return false;
                }
                __instance.collideWithPawns = false;
                bool flag = __instance.pawn.story == null || !__instance.pawn.WorkTagIsDisabled(WorkTags.Violent);
                if (flag)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        IntVec3 c = __instance.pawn.Position + GenAdj.AdjacentCellsAndInside[i];
                        if (c.InBounds(__instance.pawn.Map))
                        {
                            List<Thing> thingList = c.GetThingList(__instance.pawn.Map);
                            foreach (var thing in thingList)
                            {
                                if (flag)
                                {
                                    Pawn pawn = thing as Pawn;
                                    if (pawn != null && !pawn.Downed && __instance.pawn.HostileTo(pawn))
                                    {
                                        __instance.pawn.meleeVerbs.TryMeleeAttack(pawn, null, false);
                                        __instance.collideWithPawns = true;
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                    if (flag && __instance.pawn.Faction != null && __instance.job.def == JobDefOf.Wait_Combat && (__instance.pawn.drafter == null || __instance.pawn.drafter.FireAtWill))
                    {
                        Verb currentEffectiveVerb = __instance.pawn.CurrentEffectiveVerb;
                        if (currentEffectiveVerb != null && !currentEffectiveVerb.verbProps.IsMeleeAttack)
                        {
                            TargetScanFlags targetScanFlags = TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedLOSToNonPawns | TargetScanFlags.NeedThreat;
                            if (currentEffectiveVerb.IsIncendiary_Ranged())
                            {
                                targetScanFlags |= TargetScanFlags.NeedNonBurning;
                            }
                            Thing thing = (Thing)AttackTargetFinder.BestShootTargetFromCurrentPosition(__instance.pawn, targetScanFlags, null, 0f, 9999f);
                            if (thing != null)
                            {
                                __instance.pawn.TryStartAttack(thing);
                                __instance.collideWithPawns = true;
                                return false;
                            }
                        }
                    }
                }
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        public static bool PreNotifyPlayerOfKilled_PreFix(Pawn_HealthTracker __instance, Pawn ___pawn, ref DamageInfo? dinfo, HediffDef hediff, Caravan caravan) // disables notification if summoned Grimm disappears
        {
            if (SemblanceUtility.summonedPawnKindDefs.Any(p => p == ___pawn.RaceProps.AnyPawnKind)) return false;
            return true;
        }

        #endregion

        #region "Aura"

        public static void GeneratePawn_PostFix(Pawn __result) // adds silver eyes to a humanoid pawn
        {
            if (__result.RaceProps.Humanlike)
            {
                if (SemblanceUtility.PyrrhaMagnetismCanAffect(__result)) return; // removes Androids from birth effects
                if (__result.relations.RelatedPawns.Any(p => p.relations.Children.Contains(__result) && p.health.hediffSet.HasHediff(RWBYDefOf.RWBY_SilverEyes)) || __result.relations.Children.Any(c => c.health.hediffSet.HasHediff(RWBYDefOf.RWBY_SilverEyes)))
                {
                    if (Rand.Chance(0.5f)) // inherit with 50% chance
                    {
                        foreach (BodyPartRecord bodyPartRecord in __result.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Eye))
                        {
                            __result.health.AddHediff(RWBYDefOf.RWBY_SilverEyes, bodyPartRecord);
                        }
                    }
                }
                else if (Rand.Chance(0.01f)) // natural with 1% chance
                {
                    foreach (BodyPartRecord bodyPartRecord in __result.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Eye))
                    {
                        __result.health.AddHediff(RWBYDefOf.RWBY_SilverEyes, bodyPartRecord);
                    }
                }
            }
        }

        [HarmonyPostfix]
        public static void Notify_PawnKilled_PostFix(Pawn killed, Pawn killer) // add Weiss summon Grimm ability
        {
            if (killer.RaceProps.Humanlike && killer.story.traits.HasTrait(RWBYDefOf.Semblance_Weiss) && GrimmUtility.IsGrimm(killed) && killer.TryGetComp<CompAbilityUserAura>() is CompAbilityUserAura compAbilityUserAura && compAbilityUserAura.Initialized)
            {
                if (killed.RaceProps.AnyPawnKind == RWBYDefOf.Grimm_Boarbatusk)
                {
                    if (!compAbilityUserAura.AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_SummonBoar)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Weiss_SummonBoar);
                }
                else if (killed.RaceProps.AnyPawnKind == RWBYDefOf.Grimm_Beowolf)
                {
                    if (!compAbilityUserAura.AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_SummonBeowolf)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Weiss_SummonBeowolf);
                }
                else if (killed.RaceProps.AnyPawnKind == RWBYDefOf.Grimm_Ursa)
                {
                    if (!compAbilityUserAura.AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_SummonUrsa)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Weiss_SummonUrsa);
                }
                else if (killed.RaceProps.AnyPawnKind == RWBYDefOf.Grimm_Griffon)
                {
                    if (!compAbilityUserAura.AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_SummonGriffon)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Weiss_SummonGriffon);
                }
                else if (killed.RaceProps.AnyPawnKind == RWBYDefOf.Grimm_Nevermore)
                {
                    if (!compAbilityUserAura.AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_SummonNevermore)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Weiss_SummonNevermore);
                }
                else if (killed.RaceProps.AnyPawnKind == RWBYDefOf.Grimm_Lancer)
                {
                    if (!compAbilityUserAura.AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_SummonLancer)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Weiss_SummonLancer);
                }
                else if (killed.RaceProps.AnyPawnKind == RWBYDefOf.Grimm_LancerQueen)
                {
                    if (!compAbilityUserAura.AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_SummonLancerQueen)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Weiss_SummonLancerQueen);
                }
                else if (killed.RaceProps.AnyPawnKind == RWBYDefOf.Grimm_DeathStalker)
                {
                    if (!compAbilityUserAura.AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_SummonDeathStalker)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Weiss_SummonDeathStalker);
                }
                else if (killed.RaceProps.AnyPawnKind == RWBYDefOf.Grimm_Nuckelavee)
                {
                    if (!compAbilityUserAura.AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_SummonNuckelavee)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Weiss_SummonNuckelavee);
                }
                else if (killed.RaceProps.AnyPawnKind == RWBYDefOf.Grimm_Apathy)
                {
                    if (!compAbilityUserAura.AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_SummonApathy)) compAbilityUserAura.AddPawnAbility(RWBYDefOf.Weiss_SummonApathy);
                }
            }
        }

        [HarmonyPrefix]
        public static void TryExecuteWorker_PreFix(ref IncidentParms parms)  // may increases raid size if Semblance Qrow is present
        {
            if (parms.target is Map map && map.PlayerPawnsForStoryteller.ToList().Any(p => p.RaceProps.Humanlike && p.story.traits.HasTrait(RWBYDefOf.Semblance_Qrow)) && Rand.Chance(0.5f))
            {
                Messages.Message("MessageTextQrowRaid".Translate(), ((Map)parms.target).PlayerPawnsForStoryteller.ToList().Find(p => p.RaceProps.Humanlike && p.story.traits.HasTrait(RWBYDefOf.Semblance_Qrow)), MessageTypeDefOf.NegativeEvent);
                parms.points *= 1.2f;
            }
        }

        [HarmonyPrefix]
        public static bool AddHediff_PreFix(Hediff hediff, Pawn ___pawn)  // makes Nora immune to RimTasers Reloaded debuff and charges her
        {
            if (hediff.def.defName.Equals("Tazed") && ___pawn.story != null && ___pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Nora) && ___pawn.TryGetComp<CompAbilityUserAura>() != null && ___pawn.TryGetComp<CompAbilityUserAura>().Initialized)
            {
                var hediffCharged = HediffMaker.MakeHediff(RWBYDefOf.RWBY_LightningBuff, ___pawn);
                ___pawn.health.AddHediff(hediffCharged);
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        public static void FireEvent_PreFix(Map ___map, ref IntVec3 ___strikeLoc) // changes lightning stike location onto Nora pawns
        {
            List<Pawn> pawns = ___map.mapPawns.AllPawns.ToList().FindAll(p => p.RaceProps.Humanlike && p.story.traits.HasTrait(RWBYDefOf.Semblance_Nora));
            if (pawns.Count() > 0)
            {
                if (!___strikeLoc.IsValid)
                {
                    ___strikeLoc = CellFinderLoose.RandomCellWith((IntVec3 sq) => sq.Standable(___map) && !___map.roofGrid.Roofed(sq), ___map, 1000);
                }
                Pawn pawn;
                IntVec3 strikeLoc = ___strikeLoc;
                pawns = pawns.FindAll(p => p.Position.DistanceTo(strikeLoc) <= 30f && !p.Position.Roofed(___map));
                if (pawns.Count() > 0)
                {
                    pawn = pawns.RandomElement();
                    ___strikeLoc = pawn.Position;
                    if (pawn.TryGetComp<CompAbilityUserAura>().aura is Aura_Nora aura_Nora) aura_Nora.Notify_NextDamageIsLightning();
                }
            }
        }

        [HarmonyPrefix]
        public static bool RenderPawnAt_PreFix(PawnRenderer __instance, Pawn ___pawn, ref Vector3 drawLoc, ref Rot4? rotOverride, ref bool neverAimWeapon) // makes invisible: Ruby while dashing, Apathy while not triggered
        {
            if (___pawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_RubyDashForm)) return false;
            if (___pawn.RaceProps.AnyPawnKind == RWBYDefOf.Grimm_Apathy && !___pawn.InMentalState) return false;
            return true;
        }

        [HarmonyPostfix]
        public static void PreApplyDamage_PostFix(Pawn ___pawn, DamageInfo dinfo, ref bool absorbed) // aura absorb
        {
            if (dinfo.Def == DamageDefOf.Extinguish)
            {
                return;
            }
            if (___pawn.CurJobDef == RWBYDefOf.RWBY_StealAura) ___pawn.jobs.EndCurrentJob(JobCondition.InterruptForced); // makes the Aura steal interruptable
            if (!absorbed && ___pawn.TryGetComp<CompAbilityUserAura>() != null && ___pawn.TryGetComp<CompAbilityUserAura>().Initialized)
            {
                if (___pawn.GetComp<CompAbilityUserAura>().aura.TryAbsorbDamage(dinfo))
                {
                    absorbed = true;
                }
            }
        }

        [HarmonyPostfix]
        public static void DamageInfosToApply_PostFix(Verb_MeleeAttackDamage __instance, ref IEnumerable<DamageInfo> __result, LocalTargetInfo target) // strenghtens certain pawns melee attacks
        {
            if (!__instance.CasterIsPawn) return;
            List<DamageInfo> newOutput = new List<DamageInfo>();
            newOutput.AddRange(__result);

            if (__instance.CasterPawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_YangReturnDamage)) // add Yang damage
            {
                CompAbilityUserAura compAbilityUserAura = __instance.CasterPawn.TryGetComp<CompAbilityUserAura>();
                if (compAbilityUserAura == null) return;
                Aura_Yang aura_Yang = (Aura_Yang)compAbilityUserAura.aura;
                if (aura_Yang == null) return;
                if (aura_Yang.absorbedDamage == 0f) return;
                float damageAmount = aura_Yang.absorbedDamage;

                DamageInfo extraDinfo = new DamageInfo(DamageDefOf.Blunt, damageAmount, 0f, -1f, __instance.caster, null, __instance.CasterPawn.def, DamageInfo.SourceCategory.ThingOrUnknown, null);
                extraDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                BodyPartGroupDef bodyPartGroupDef = __instance.verbProps.AdjustedLinkedBodyPartsGroup(__instance.tool);
                extraDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
                HediffDef hediffDef = null;
                if (__instance.HediffCompSource != null)
                {
                    hediffDef = __instance.HediffCompSource.Def;
                }
                extraDinfo.SetWeaponHediff(hediffDef);
                Vector3 direction = (target.Thing.Position - __instance.CasterPawn.Position).ToVector3();
                extraDinfo.SetAngle(direction);
                newOutput.Add(extraDinfo);
            }
            if (__instance.CasterPawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_LightningBuff)) // add Nora damage
            {
                float damageAmount = 20f;

                DamageInfo extraDinfo = new DamageInfo(RWBYDefOf.RWBY_Lightning_Blunt, damageAmount, 0f, -1f, __instance.caster, null, __instance.CasterPawn.def, DamageInfo.SourceCategory.ThingOrUnknown, null);
                extraDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                BodyPartGroupDef bodyPartGroupDef = __instance.verbProps.AdjustedLinkedBodyPartsGroup(__instance.tool);
                extraDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
                HediffDef hediffDef = null;
                if (__instance.HediffCompSource != null)
                {
                    hediffDef = __instance.HediffCompSource.Def;
                }
                extraDinfo.SetWeaponHediff(hediffDef);
                Vector3 direction = (target.Thing.Position - __instance.CasterPawn.Position).ToVector3();
                extraDinfo.SetAngle(direction);
                newOutput.Add(extraDinfo);
            }

            if (__instance.CasterPawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_InjectedFireCrystal)) // add Fire Dust Crystal injection damage
            {
                float damageAmount = 20f + 10f * __instance.CasterPawn.health.hediffSet.hediffs.Find(h => h.def == RWBYDefOf.RWBY_InjectedFireCrystal).CurStageIndex;

                DamageInfo extraDinfo = new DamageInfo(RWBYDefOf.RWBY_Inflame_Blunt, damageAmount, 0f, -1f, __instance.caster, null, __instance.CasterPawn.def, DamageInfo.SourceCategory.ThingOrUnknown, null);
                extraDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                BodyPartGroupDef bodyPartGroupDef = __instance.verbProps.AdjustedLinkedBodyPartsGroup(__instance.tool);
                extraDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
                HediffDef hediffDef = null;
                if (__instance.HediffCompSource != null)
                {
                    hediffDef = __instance.HediffCompSource.Def;
                }
                extraDinfo.SetWeaponHediff(hediffDef);
                Vector3 direction = (target.Thing.Position - __instance.CasterPawn.Position).ToVector3();
                extraDinfo.SetAngle(direction);
                newOutput.Add(extraDinfo);
            }
            if (__instance.CasterPawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_InjectedIceCrystal)) // add Ice Dust Crystal injection damage
            {
                float damageAmount = 20f + 10f * __instance.CasterPawn.health.hediffSet.hediffs.Find(h => h.def == RWBYDefOf.RWBY_InjectedIceCrystal).CurStageIndex;

                DamageInfo extraDinfo = new DamageInfo(RWBYDefOf.RWBY_Ice_Blunt, damageAmount, 0f, -1f, __instance.caster, null, __instance.CasterPawn.def, DamageInfo.SourceCategory.ThingOrUnknown, null);
                extraDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                BodyPartGroupDef bodyPartGroupDef = __instance.verbProps.AdjustedLinkedBodyPartsGroup(__instance.tool);
                extraDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
                HediffDef hediffDef = null;
                if (__instance.HediffCompSource != null)
                {
                    hediffDef = __instance.HediffCompSource.Def;
                }
                extraDinfo.SetWeaponHediff(hediffDef);
                Vector3 direction = (target.Thing.Position - __instance.CasterPawn.Position).ToVector3();
                extraDinfo.SetAngle(direction);
                newOutput.Add(extraDinfo);
            }
            if (__instance.CasterPawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_InjectedLightningCrystal)) // add Lightning Dust Crystal injection damage
            {
                float damageAmount = 20f + 10f * __instance.CasterPawn.health.hediffSet.hediffs.Find(h => h.def == RWBYDefOf.RWBY_InjectedLightningCrystal).CurStageIndex;

                DamageInfo extraDinfo = new DamageInfo(RWBYDefOf.RWBY_Lightning_Blunt, damageAmount, 0f, -1f, __instance.caster, null, __instance.CasterPawn.def, DamageInfo.SourceCategory.ThingOrUnknown, null);
                extraDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                BodyPartGroupDef bodyPartGroupDef = __instance.verbProps.AdjustedLinkedBodyPartsGroup(__instance.tool);
                extraDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
                HediffDef hediffDef = null;
                if (__instance.HediffCompSource != null)
                {
                    hediffDef = __instance.HediffCompSource.Def;
                }
                extraDinfo.SetWeaponHediff(hediffDef);
                Vector3 direction = (target.Thing.Position - __instance.CasterPawn.Position).ToVector3();
                extraDinfo.SetAngle(direction);
                newOutput.Add(extraDinfo);
            }
            if (__instance.CasterPawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_InjectedGravityCrystal)) // add Gravity Dust Crystal injection damage
            {
                float damageAmount = 20f + 10f * __instance.CasterPawn.health.hediffSet.hediffs.Find(h => h.def == RWBYDefOf.RWBY_InjectedGravityCrystal).CurStageIndex;

                DamageInfo extraDinfo = new DamageInfo(DamageDefOf.Blunt, damageAmount, 0f, -1f, __instance.caster, null, __instance.CasterPawn.def, DamageInfo.SourceCategory.ThingOrUnknown, null);
                extraDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                BodyPartGroupDef bodyPartGroupDef = __instance.verbProps.AdjustedLinkedBodyPartsGroup(__instance.tool);
                extraDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
                HediffDef hediffDef = null;
                if (__instance.HediffCompSource != null)
                {
                    hediffDef = __instance.HediffCompSource.Def;
                }
                extraDinfo.SetWeaponHediff(hediffDef);
                Vector3 direction = (target.Thing.Position - __instance.CasterPawn.Position).ToVector3();
                extraDinfo.SetAngle(direction);
                newOutput.Add(extraDinfo);
            }
            if (__instance.CasterPawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_InjectedHardLightCrystal)) // add HardLight Dust Crystal injection damage
            {
                float damageAmount = 20f + 10f * __instance.CasterPawn.health.hediffSet.hediffs.Find(h => h.def == RWBYDefOf.RWBY_InjectedHardLightCrystal).CurStageIndex;

                DamageInfo extraDinfo = new DamageInfo(RWBYDefOf.RWBY_Burn_Blunt, damageAmount, 0f, -1f, __instance.caster, null, __instance.CasterPawn.def, DamageInfo.SourceCategory.ThingOrUnknown, null);
                extraDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                BodyPartGroupDef bodyPartGroupDef = __instance.verbProps.AdjustedLinkedBodyPartsGroup(__instance.tool);
                extraDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
                HediffDef hediffDef = null;
                if (__instance.HediffCompSource != null)
                {
                    hediffDef = __instance.HediffCompSource.Def;
                }
                extraDinfo.SetWeaponHediff(hediffDef);
                Vector3 direction = (target.Thing.Position - __instance.CasterPawn.Position).ToVector3();
                extraDinfo.SetAngle(direction);
                newOutput.Add(extraDinfo);
            }

            __result = newOutput;
        }

        #endregion

        #region "Items"

        [HarmonyPostfix]
        public static void Ingested_PostFix(ref float __result, Thing __instance, Pawn ingester) // checks for Pumpkin Pete´s eaten
        {
            if (__instance.def == RWBYDefOf.RWBY_PumpkinPetes && __result != 0)
            {
                if (ingester.TryGetComp<CompAbilityUserAura>() != null)
                {
                    ingester.TryGetComp<CompAbilityUserAura>().Notify_EatenPumkinPetes();

                    if (ingester.story != null && ingester.story.traits.HasTrait(RWBYDefOf.Semblance_Jaune))
                    {
                        Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(RWBYDefOf.RWBY_JauneAtePumpkinPetes);
                        ingester.needs.mood.thoughts.memories.TryGainMemory(thought_Memory);
                    }
                }
            }
        }

        [HarmonyPostfix]
        public static void ExplosionAffectCell_PostFix(DamageWorker_Flame __instance, Explosion explosion, IntVec3 c, List<Thing> damagedThings, bool canThrowMotes) // makes fire Dust spawn fire on explosion
        {
            if (__instance.def == RWBYDefOf.Bomb_Fire && Rand.Chance(FireUtility.ChanceToStartFireIn(c, explosion.Map)))
            {
                FireUtility.TryStartFireIn(c, explosion.Map, Rand.Range(0.2f, 0.6f));
            }
        }

        [HarmonyPostfix]
        public static void TryDropSpawn_PostFix(Thing thing) // lets light copies disappear on drop
        {
            if (thing != null && thing.GetType().Equals(typeof(ThingWithComps)) && ((ThingWithComps)thing).TryGetComp<CompLightCopy>() != null)
            {
                thing.Destroy();
            }
        }

        [HarmonyPostfix]
        public static void GetGizmos_PostFix(Pawn_EquipmentTracker __instance, ref IEnumerable<Gizmo> __result) // adds abilities to pawns
        {
            Pawn pawn = __instance.pawn;
            if (!pawn.IsColonist) return;
            if (PawnAttackGizmoUtility.CanShowEquipmentGizmos())
            {
                List<Gizmo> newOutput = new List<Gizmo>();
                newOutput.AddRange(__result);

                if (pawn.Drafted)
                {
                    foreach (ThingWithComps thingWithComps in __instance.AllEquipmentListForReading)
                    {
                        foreach (ThingComp thingComp in thingWithComps.AllComps.FindAll(c => c is CompWeaponTransform || c is CompWeaponProjectile || c is CompWeaponDrinkCoffee || c is CompTakePhoto || c is CompStealAura))
                        {
                            newOutput.AddRange(thingComp.CompGetGizmosExtra());
                        }
                    }
                }
                else
                {
                    if (pawn.equipment.Primary != null && pawn.equipment.Primary.TryGetComp<CompLightCopy>() != null)
                    {
                        pawn.equipment.Primary.Destroy();
                    }
                }
                if (pawn.inventory.innerContainer.Count > 0)
                {
                    foreach (Thing thing in pawn.inventory.innerContainer)
                    {
                        if (thing is ThingWithComps thingWithComps)
                        {
                            foreach (ThingComp thingComp in thingWithComps.AllComps.FindAll(c => c is CompWeaponProjectile || c is CompInjectDustCrystal))
                            {
                                newOutput.AddRange(thingComp.CompGetGizmosExtra());
                            }
                        }
                    }
                }
                __result = newOutput;
            }
        }

        #endregion

        #region "special"

        [HarmonyPostfix]
        public static void TipString_PostFix(Trait __instance, ref string __result) // adds disabled working tags to Trait descriptions
        {
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().fixTraitDisabledWorkTags) return;
            StringBuilder stringBuilder = new StringBuilder();

            if (__instance.def.disabledWorkTags != 0)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                foreach (WorkTags workTag in __instance.def.disabledWorkTags.GetAllSelectedItems<WorkTags>())
                {
                    if (workTag != 0) stringBuilder.AppendLine("    " + workTag.LabelTranslated().CapitalizeFirst() + " " + "disabled");
                }
                __result += stringBuilder.ToString();
            }
        }

        [HarmonyPrefix]
        public static bool ProcessInputEvents_Prefix(Targeter __instance) // lets the weapon projectile ability aim properly
        {
            if (__instance.targetingSource is Verb_ShootWeaponAbility verb_ShootWeaponAbility)
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && __instance.IsTargeting)
                {
                    var obj = (LocalTargetInfo)AccessTools.Method(typeof(Targeter), "CurrentTargetUnderMouse").Invoke(__instance, new object[] { false });
                    if (obj.IsValid)
                    {
                        Pawn pawn = verb_ShootWeaponAbility.CasterPawn;
                        Job job = JobMaker.MakeJob(RWBYDefOf.RWBY_ShootProjectileAbility, obj);
                        job.verbToUse = verb_ShootWeaponAbility;
                        pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                        SoundDefOf.Tick_High.PlayOneShotOnCamera();
                        __instance.StopTargeting();
                        Event.current.Use();
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion
    }
}
