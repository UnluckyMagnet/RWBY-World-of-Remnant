using Verse;
using Harmony;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using RimWorld.Planet;
using System.Linq;
using Verse.AI;
using System;
using System.Text;

namespace RWBYRemnant
{
    [StaticConstructorOnStartup]
    static class HarmonyPatch
    {
        static HarmonyPatch()
        {
            var harmony = HarmonyInstance.Create("rimworld.carnysenpai.rwbyremnant");
            harmony.Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), "GetGizmos"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("GetGizmos_PostFix")), null); // adds weapon abilities to pawns
            harmony.Patch(AccessTools.Method(typeof(GenDrop), "TryDropSpawn"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("TryDropSpawn_PostFix")), null); // lets light copies disappear on drop
            harmony.Patch(AccessTools.Method(typeof(Verb_MeleeAttackDamage), "DamageInfosToApply"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("DamageInfosToApply_PostFix")), null); // strenghtens certain pawns melee attacks
            harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "PreApplyDamage"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("PreApplyDamage_PostFix")), null); // aura absorb
            harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "NotifyPlayerOfKilled"), new HarmonyMethod(typeof(HarmonyPatch).GetMethod("PreNotifyPlayerOfKilled_PreFix")), null, null); // disables notification if summoned Grimm disappears
            harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "AddHediff", new[] { typeof(Hediff), typeof(BodyPartRecord), typeof(DamageInfo), typeof(DamageWorker.DamageResult) }), new HarmonyMethod(typeof(HarmonyPatch).GetMethod("AddHediff_PreFix")), null, null);  // makes Nora immune to RimTasers Reloaded debuff and charges her
            harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "RenderPawnAt", new[] { typeof(Vector3) }), new HarmonyMethod(typeof(HarmonyPatch).GetMethod("RenderPawnAt_PreFix")), null, null); // makes Ruby invisible while dashing
            harmony.Patch(AccessTools.Method(typeof(DamageWorker_Flame), "ExplosionAffectCell"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("ExplosionAffectCell_PostFix")), null); // makes fire Dust spawn fire on explosion
            harmony.Patch(AccessTools.Method(typeof(JobDriver_Wait), "CheckForAutoAttack"), new HarmonyMethod(typeof(HarmonyPatch).GetMethod("CheckForAutoAttack_PreFix")), null, null); // fixes summoned Grimm bug of nullpointer if wandering
            harmony.Patch(AccessTools.Method(typeof(WeatherEvent_LightningStrike), "FireEvent"), new HarmonyMethod(typeof(HarmonyPatch).GetMethod("FireEvent_PreFix")), null, null); // changes lightning stike location onto Nora pawns
            harmony.Patch(AccessTools.Method(typeof(WeatherEvent_LightningStrike), "FireEvent"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("FireEvent_PostFix")), null); // unlocks Semblance Nora
            harmony.Patch(AccessTools.Method(typeof(Thing), "Ingested"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("Ingested_PostFix")), null); // checks for Pumpkin Pete´s eaten
            harmony.Patch(AccessTools.Method(typeof(Pawn_RecordsTracker), "Increment"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("Increment_PostFix")), null); // unlocks Semblance Jaune Pyrrha Cinder
            harmony.Patch(AccessTools.Method(typeof(Pawn_RecordsTracker), "AddTo"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("AddTo_PostFix")), null); // unlocks Semblance Yang
            harmony.Patch(AccessTools.Method(typeof(Thing), "TakeDamage"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("TakeDamage_PostFix")), null); // unlocks Semblance Ruby
            harmony.Patch(AccessTools.Method(typeof(Pawn_JobTracker), "StartJob"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("StartJob_PostFix")), null); // unlocks Semblance Blake
            harmony.Patch(AccessTools.Method(typeof(IncidentWorker_Raid), "TryExecuteWorker"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("TryExecuteWorker_PostFix")), null); // unlocks Semblance Ren
            harmony.Patch(AccessTools.Method(typeof(RecordsUtility), "Notify_PawnKilled"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("Notify_PawnKilled_PostFix")), null); // unlocks Semblance Weiss
            harmony.Patch(AccessTools.Method(typeof(HealthUtility), "GiveRandomSurgeryInjuries"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("GiveRandomSurgeryInjuries_PostFix")), null); // unlocks Semblance Qrow
            harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "MakeDowned"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("MakeDowned_PostFix")), null); // unlock Semblance Hazel Raven
            harmony.Patch(AccessTools.Method(typeof(Trait), "TipString"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("TipString_PostFix")), null); // adds disabled working tags to Trait descriptions
            harmony.Patch(AccessTools.Method(typeof(GenHostility), "HostileTo", new[] { typeof(Thing), typeof(Thing) }), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("HostileTo_PostFix")), null); // makes Grimm unable to attack pawns affected by Ren or without negative emotions
            harmony.Patch(AccessTools.Method(typeof(IncidentWorker_RaidEnemy), "TryExecuteWorker"), new HarmonyMethod(typeof(HarmonyPatch).GetMethod("TryExecuteWorker_PreFix")), null, null);  // may increases raid size if Semblance Qrow is present
            harmony.Patch(AccessTools.Method(typeof(AttackTargetFinder), "BestAttackTarget"), new HarmonyMethod(typeof(HarmonyPatch).GetMethod("BestAttackTarget_PreFix")), null, null); // makes Grimm not need line of sight
            harmony.Patch(AccessTools.Method(typeof(Pawn_InteractionsTracker), "TryInteractWith"), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("TryInteractWith_PostFix")), null); // unlock Semblance Velvet
        }

        #region "unlock Semblances"

        [HarmonyPostfix]
        public static void TryInteractWith_PostFix(Pawn ___pawn, Pawn recipient, bool __result) // unlock Semblance Velvet
        {
            if (!__result) return;
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().semblanceUnlockable) return;
            if (___pawn.RaceProps.Humanlike && recipient.RaceProps.Humanlike && ___pawn.story != null && ___pawn.story.traits.HasTrait(RWBYDefOf.RWBY_Aura) && ___pawn.Faction == Faction.OfPlayer)
            {
                if (Rand.Chance(0.003f))
                {
                    SemblanceUtility.UnlockSemblance(___pawn, RWBYDefOf.Semblance_Velvet, "LetterTextUnlockSemblanceVelvet");
                }
            }
        }

        [HarmonyPostfix]
        public static void MakeDowned_PostFix(Pawn ___pawn) // unlock Semblance Hazel Raven
        {
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().semblanceUnlockable) return;
            if (___pawn.health.InPainShock && ___pawn.story != null && ___pawn.story.traits.HasTrait(RWBYDefOf.RWBY_Aura) && ___pawn.Faction == Faction.OfPlayer)
            {
                if (Rand.Chance(0.03f))
                {
                    SemblanceUtility.UnlockSemblance(___pawn, RWBYDefOf.Semblance_Hazel, "LetterTextUnlockSemblanceHazel");
                    Hediff hediffIgnorePain = new Hediff();
                    hediffIgnorePain = HediffMaker.MakeHediff(RWBYDefOf.RWBY_IgnorePain, ___pawn);
                    ___pawn.health.AddHediff(hediffIgnorePain);
                }
            }
            if (___pawn.Map != null && ___pawn.Map.PlayerPawnsForStoryteller.ToList().Any(p => p != ___pawn && p.story != null && p.story.traits.HasTrait(RWBYDefOf.RWBY_Aura) && p.relations.OpinionOf(___pawn) >= 30))
            {
                if (Rand.Chance(0.1f))
                {
                    List<Pawn> pawns = ___pawn.Map.PlayerPawnsForStoryteller.ToList().FindAll(p => p != ___pawn && p.story != null && p.story.traits.HasTrait(RWBYDefOf.RWBY_Aura) && p.relations.OpinionOf(___pawn) >= 30);
                    Pawn pawn = pawns.RandomElement();
                    if (SemblanceUtility.UnlockSemblance(pawn, RWBYDefOf.Semblance_Raven, "LetterTextUnlockSemblanceRaven"))
                    {
                        if (pawn.TryGetComp<CompAbilityUserAura>().aura is Aura_Raven aura_Raven)
                        {
                            aura_Raven.RegisterBondedPawn(___pawn);
                        }
                    }
                }
            }
        }

        [HarmonyPostfix]
        public static void GiveRandomSurgeryInjuries_PostFix(Pawn p) // unlocks Semblance Qrow
        {
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().semblanceUnlockable) return;
            if (p.story != null && p.story.traits.HasTrait(RWBYDefOf.RWBY_Aura) && p.Faction == Faction.OfPlayer)
            {
                if (Rand.Chance(0.1f)) SemblanceUtility.UnlockSemblance(p, RWBYDefOf.Semblance_Qrow, "LetterTextUnlockSemblanceQrow");
            }
        }

        [HarmonyPostfix]
        public static void Notify_PawnKilled_PostFix(Pawn killed, Pawn killer) // unlocks Semblance Weiss
        {
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().semblanceUnlockable) return;
            if (killer.story != null && killer.story.traits.HasTrait(RWBYDefOf.RWBY_Aura) && killer.Faction == Faction.OfPlayer && killed.Faction != null && killed.Faction.def == RWBYDefOf.Creatures_of_Grimm)
            {
                if (Rand.Chance(0.02f)) SemblanceUtility.UnlockSemblance(killer, RWBYDefOf.Semblance_Weiss, "LetterTextUnlockSemblanceWeiss");
            }
        }

        [HarmonyPostfix]
        public static void TryExecuteWorker_PostFix(IncidentWorker_Raid __instance, IncidentParms parms) // unlocks Semblance Ren
        {
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().semblanceUnlockable) return;
            if (parms.faction == null || parms.faction.def != RWBYDefOf.Creatures_of_Grimm) return;
            List<Pawn> pawns = ((Map)parms.target).PlayerPawnsForStoryteller.ToList().FindAll(p => p.RaceProps.Humanlike && p.story.traits.HasTrait(RWBYDefOf.RWBY_Aura));
            if (pawns.Count() > 0 && Rand.Chance(0.2f)) SemblanceUtility.UnlockSemblance(pawns.RandomElement(), RWBYDefOf.Semblance_Ren, "LetterTextUnlockSemblanceRen");
        }

        [HarmonyPostfix]
        public static void FireEvent_PostFix(WeatherEvent_LightningStrike __instance, Map ___map, IntVec3 ___strikeLoc, Mesh ___boltMesh) // unlocks Semblance Nora
        {
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().semblanceUnlockable) return;
            List<Pawn> pawns = ___map.PlayerPawnsForStoryteller.ToList().FindAll(p => p.RaceProps.Humanlike);
            foreach (Pawn pawn in pawns)
            {
                if (pawn.Position.DistanceTo(___strikeLoc) <= 1.1f && pawn.story.traits.HasTrait(RWBYDefOf.RWBY_Aura))
                {
                    SemblanceUtility.UnlockSemblance(pawn, RWBYDefOf.Semblance_Nora, "LetterTextUnlockSemblanceNora");
                    break;
                }
            }
        }

        [HarmonyPostfix]
        public static void StartJob_PostFix(Pawn_JobTracker __instance, Job newJob, Pawn ___pawn) // unlocks Semblance Blake
        {
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().semblanceUnlockable) return;
            if (___pawn.story != null && ___pawn.story.traits.HasTrait(RWBYDefOf.RWBY_Aura) && ___pawn.Faction == Faction.OfPlayer)
            {
                if (newJob.def == JobDefOf.Flee || newJob.def == JobDefOf.FleeAndCower)
                {
                    if (Rand.Chance(0.05f)) SemblanceUtility.UnlockSemblance(___pawn, RWBYDefOf.Semblance_Blake, "LetterTextUnlockSemblanceBlake");
                }
            }
        }

        [HarmonyPostfix]
        public static void Increment_PostFix(Pawn_RecordsTracker __instance, RecordDef def) // unlocks Semblance Jaune Pyrrha Cinder
        {
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().semblanceUnlockable) return;
            if (__instance.pawn.story != null && __instance.pawn.story.traits.HasTrait(RWBYDefOf.RWBY_Aura) && __instance.pawn.Faction == Faction.OfPlayer)
            {
                if (def == RecordDefOf.TimesTendedOther)
                {
                    if (Rand.Chance(0.02f)) SemblanceUtility.UnlockSemblance(__instance.pawn, RWBYDefOf.Semblance_Jaune, "LetterTextUnlockSemblanceJaune");
                }
                else if (def == RecordDefOf.KillsMechanoids || def == RecordDefOf.PawnsDownedMechanoids)
                {
                    if (Rand.Chance(0.02f)) SemblanceUtility.UnlockSemblance(__instance.pawn, RWBYDefOf.Semblance_Pyrrha, "LetterTextUnlockSemblancePyrrha");
                }
                else if (def == RecordDefOf.TimesOnFire)
                {
                    if (Rand.Chance(0.05f)) SemblanceUtility.UnlockSemblance(__instance.pawn, RWBYDefOf.Semblance_Cinder, "LetterTextUnlockSemblanceCinder");
                }
            }
        }

        [HarmonyPostfix]
        public static void AddTo_PostFix(Pawn_RecordsTracker __instance, RecordDef def, float value) // unlocks Semblance Yang
        {
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().semblanceUnlockable) return;
            if (__instance.pawn.story != null && __instance.pawn.story.traits.HasTrait(RWBYDefOf.RWBY_Aura) && __instance.pawn.Faction == Faction.OfPlayer)
            {
                if (def == RecordDefOf.DamageTaken)
                {
                    if (Rand.Chance(0.005f * Math.Min(1f, (value / 20f)))) SemblanceUtility.UnlockSemblance(__instance.pawn, RWBYDefOf.Semblance_Yang, "LetterTextUnlockSemblanceYang");
                }
            }
        }

        [HarmonyPostfix]
        public static void TakeDamage_PostFix(Thing __instance, DamageInfo dinfo) // unlocks Semblance Ruby
        {
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().semblanceUnlockable) return;
            if (__instance is Pawn pawn && pawn.RaceProps.intelligence != Intelligence.Animal && dinfo.Instigator is Pawn instigatorPawn && instigatorPawn.story != null && instigatorPawn.story.traits.HasTrait(RWBYDefOf.RWBY_Aura) && instigatorPawn.Faction == Faction.OfPlayer)
            {
                if (Rand.Chance(0.005f * Math.Min(1f, (dinfo.Amount / 40f)))) SemblanceUtility.UnlockSemblance(instigatorPawn, RWBYDefOf.Semblance_Ruby, "LetterTextUnlockSemblanceRuby");
            }
        }

        #endregion

        #region "Grimm"

        [HarmonyPostfix]
        public static void HostileTo_PostFix(ref bool __result, Thing a, Thing b) // makes Grimm unable to attack pawns affected by Ren or without negative emotions
        {
            if (!__result) return;
            if (!(a is Pawn searcherPawn)) return;
            if (searcherPawn.RaceProps.FleshType != RWBYDefOf.Grimm || searcherPawn.Faction.def != RWBYDefOf.Creatures_of_Grimm) return;
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
            if (searcherPawn.RaceProps.FleshType != RWBYDefOf.Grimm || searcherPawn.Faction.def != RWBYDefOf.Creatures_of_Grimm) return;
            flags = TargetScanFlags.None;
        }

        [HarmonyPrefix]
        public static bool CheckForAutoAttack_PreFix(JobDriver_Wait __instance) // fixes summoned Grimm bug of nullpointer if wandering
        {
            if (__instance.pawn.RaceProps.FleshType == RWBYDefOf.Grimm)
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
                bool flag = __instance.pawn.story == null || !__instance.pawn.story.WorkTagIsDisabled(WorkTags.Violent);
                if (flag)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        IntVec3 c = __instance.pawn.Position + GenAdj.AdjacentCellsAndInside[i];
                        if (c.InBounds(__instance.pawn.Map))
                        {
                            List<Thing> thingList = c.GetThingList(__instance.pawn.Map);
                            for (int j = 0; j < thingList.Count; j++)
                            {
                                if (flag)
                                {
                                    Pawn pawn = thingList[j] as Pawn;
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
                            if (currentEffectiveVerb.IsIncendiary())
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

        [HarmonyPrefix]
        public static void TryExecuteWorker_PreFix(ref IncidentParms parms)  // may increases raid size if Semblance Qrow is present
        {
            if (((Map)parms.target).PlayerPawnsForStoryteller.ToList().Any(p => p.RaceProps.Humanlike && p.story.traits.HasTrait(RWBYDefOf.Semblance_Qrow)) && Rand.Chance(0.5f))
            {
                Messages.Message("MessageTextQrowRaid".Translate(), ((Map)parms.target).PlayerPawnsForStoryteller.ToList().Find(p => p.RaceProps.Humanlike && p.story.traits.HasTrait(RWBYDefOf.Semblance_Qrow)), MessageTypeDefOf.NegativeEvent);
                parms.points *= 1.2f;
            }
        }

        [HarmonyPrefix]
        public static bool AddHediff_PreFix(Hediff hediff, Pawn ___pawn)  // makes Nora immune to RimTasers Reloaded debuff and charges her
        {
            if (hediff.def.defName.Equals("Tazed") && ___pawn.story != null && ___pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Nora) && ___pawn.TryGetComp<CompAbilityUserAura>() != null && ___pawn.TryGetComp<CompAbilityUserAura>().IsInitialized)
            {
                Hediff hediffCharged = new Hediff();
                hediffCharged = HediffMaker.MakeHediff(RWBYDefOf.RWBY_LightningBuff, ___pawn);
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
        public static bool RenderPawnAt_PreFix(PawnRenderer __instance, Pawn ___pawn, ref Vector3 drawLoc) // makes Ruby invisible while dashing
        {
            if (___pawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_RubyDashForm))
            {
                return false;
            }
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
            if (!absorbed && ___pawn.TryGetComp<CompAbilityUserAura>() != null && ___pawn.TryGetComp<CompAbilityUserAura>().IsInitialized)
            {
                if (___pawn.story.traits.HasTrait(RWBYDefOf.RWBY_Aura) && (dinfo.Def == DamageDefOf.EMP || dinfo.Def == RWBYDefOf.Bomb_Lightning || dinfo.Def == RWBYDefOf.RWBY_Lightning_Slash || dinfo.Def == RWBYDefOf.RWBY_Lightning_Blunt || dinfo.Def == RWBYDefOf.RWBY_Lightning_Bullet || SemblanceUtility.noraDmgAbsorbDefs.Contains(dinfo.Def.defName)))
                {
                    if (Rand.Chance(0.01f))
                    {
                        SemblanceUtility.UnlockSemblance(___pawn, RWBYDefOf.Semblance_Nora, "LetterTextUnlockSemblanceNora");
                        absorbed = true;
                        return;
                    }
                }
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
            if (thing != null && thing.GetType().Equals(typeof(ThingWithComps)) && ((ThingWithComps)thing).TryGetComp<LightCopyDestroyAbility>() != null)
            {
                thing.Destroy(DestroyMode.Vanish);
            }
        }

        [HarmonyPostfix]
        public static void GetGizmos_PostFix(Pawn_EquipmentTracker __instance, ref IEnumerable<Gizmo> __result) // adds weapon abilities to pawns
        {
            Pawn pawn = __instance.pawn;
            if (!pawn.IsColonist) return;
            if (PawnAttackGizmoUtility.CanShowEquipmentGizmos())
            {
                List<Gizmo> newOutput = new List<Gizmo>();
                newOutput.AddRange(__result);

                if (pawn.Drafted)
                {
                    IEnumerator<ThingWithComps> enumerator = __instance.AllEquipmentListForReading.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ThingWithComps current = enumerator.Current;
                        if (current.def.defName.Contains("RWBY"))
                        {
                            IEnumerator<Gizmo> compGizmosEnumerator = current.GetGizmos().GetEnumerator();
                            while (compGizmosEnumerator.MoveNext())
                            {
                                Gizmo currentGizmo = compGizmosEnumerator.Current;
                                if (((Command)currentGizmo) != null && !((Command)currentGizmo).Label.Equals("CommandForbid".Translate()) && !((Command)currentGizmo).Label.Equals("CommandAllow".Translate()))
                                {
                                    newOutput.Add(currentGizmo);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (pawn.equipment.Primary != null && pawn.equipment.Primary.TryGetComp<LightCopyDestroyAbility>() != null)
                    {
                        pawn.equipment.Primary.Destroy();
                    }
                }
                if (pawn.inventory.innerContainer.Count > 0)
                {
                    foreach (Thing thing in pawn.inventory.innerContainer)
                    {
                        foreach (Gizmo gizmo in thing.GetGizmos())
                        {
                            if (((Command)gizmo) != null && !((Command)gizmo).Label.Equals("CommandForbid".Translate()) && !((Command)gizmo).Label.Equals("CommandAllow".Translate()))
                            {
                                newOutput.Add(gizmo);
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
                __result = __result + stringBuilder.ToString();
            }
        }

        #endregion
    }
}
