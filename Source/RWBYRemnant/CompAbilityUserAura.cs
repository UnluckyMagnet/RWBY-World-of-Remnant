using RimWorld;
using System.Collections.Generic;
using Verse;
using AbilityUser;
using System.Linq;

namespace RWBYRemnant
{
    public class CompAbilityUserAura : CompAbilityUser
    {
        public Aura aura;

        public override void CompTick()
        {
            base.CompTick();
            if (Pawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_AuraStolen)) return;
            if (hiddenSemblance == null) GenerateHiddenSemblance();
            if (Initialized)
            {
                aura.Tick();
            }
            else
            {
                if (auraAutoUnlock >= 0) auraAutoUnlock--;
                if (auraAutoUnlock == 0)
                {
                    SemblanceUtility.UnlockAura(Pawn, "LetterTextUnlockAuraAuto");
                }
            }
        }

        public override void CompTickRare()
        {
            base.CompTickRare();            
            if (!Pawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_SilverEyes) && AbilityData.AllPowers.Any(a => a.Def == RWBYDefOf.Ability_SilverEyes))
            {
                RemovePawnAbility(RWBYDefOf.Ability_SilverEyes);
                Messages.Message("MessageTextLostSilverEyes".Translate().Formatted(Pawn.Named("PAWN")).AdjustedFor(Pawn, "PAWN").CapitalizeFirst(), Pawn, MessageTypeDefOf.NegativeEvent);
            }
            if (Pawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_AuraStolen))
            {
                Initialized = false;
                aura = null;
                AbilityData.AllPowers.Clear();
                return;
            }
            if (Initialized)
            {
                aura.TickRare();
                if (!Pawn.story.traits.allTraits.Any(t => t.def == RWBYDefOf.RWBY_Aura || SemblanceUtility.semblanceList.Contains(t.def)) && !LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().everyoneHasAura)
                {
                    Initialized = false;
                    aura = null;
                    AbilityData.AllPowers.Clear();
                }
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (aura != null)
            {
                aura.Draw();
            }
        }     

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (Find.Selector.NumSelected > 1 && LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().hideAuraWhenMultiselect) yield break;
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            if (Initialized)
            {
                yield return new GizmoAuraStatus
                {
                    aura = aura,
                    label = "AuraGizmoLabel".Translate(Pawn.Name.ToStringShort),
                    FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(aura.GetColor())
                };
                foreach (Gizmo gizmo in aura.GetGizmos())
                {
                    yield return gizmo;
                }
            }
        }

        public override void PostInitialize()
        {
            if (Pawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_SilverEyes)) // add silver eye ability if pawn has unlocked an Aura
            {
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Ability_SilverEyes)) AddPawnAbility(RWBYDefOf.Ability_SilverEyes);
            }

            if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Ruby)) // Ruby
            {
                aura = new Aura_Ruby
                {
                    pawn = Pawn,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Ruby_BurstIntoRosePetals)) AddPawnAbility(RWBYDefOf.Ruby_BurstIntoRosePetals);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Ruby_CarryPawn)) AddPawnAbility(RWBYDefOf.Ruby_CarryPawn);
            }
            else if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Yang)) // Yang
            {
                aura = new Aura_Yang
                {
                    pawn = Pawn,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
                AddPawnAbility(RWBYDefOf.Yang_ReturnDamage);
            }
            else if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Weiss)) // Weiss
            {
                aura = new Aura_Weiss
                {
                    pawn = Pawn,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_TimeDilationGlyph_Summon)) AddPawnAbility(RWBYDefOf.Weiss_TimeDilationGlyph_Summon);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_SummonArmaGigas)) AddPawnAbility(RWBYDefOf.Weiss_SummonArmaGigas);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_SummonArmaGigasSword)) AddPawnAbility(RWBYDefOf.Weiss_SummonArmaGigasSword);
            }
            else if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Blake)) // Blake
            {
                aura = new Aura_Blake
                {
                    pawn = Pawn,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Blake_UseStoneClone)) AddPawnAbility(RWBYDefOf.Blake_UseStoneClone);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Blake_UseIceClone)) AddPawnAbility(RWBYDefOf.Blake_UseIceClone);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Blake_UseFireClone)) AddPawnAbility(RWBYDefOf.Blake_UseFireClone);
            }
            else if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Nora)) // Nora
            {
                aura = new Aura_Nora
                {
                    pawn = Pawn,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
            }
            else if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Jaune)) // Jaune
            {
                aura = new Aura_Jaune
                {
                    pawn = Pawn,
                    maxEnergy = 1.5f,
                    currentEnergy = 1.5f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Jaune_AmplifyAura)) AddPawnAbility(RWBYDefOf.Jaune_AmplifyAura);
            }
            else if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Pyrrha)) // Pyrrha
            {
                aura = new Aura_Pyrrha
                {
                    pawn = Pawn,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Pyrrha_UnlockAura)) AddPawnAbility(RWBYDefOf.Pyrrha_UnlockAura);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Pyrrha_UseMagnetism)) AddPawnAbility(RWBYDefOf.Pyrrha_UseMagnetism);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Pyrrha_MagneticPulse)) AddPawnAbility(RWBYDefOf.Pyrrha_MagneticPulse);
            }
            else if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Ren)) // Ren
            {
                aura = new Aura_Ren
                {
                    pawn = Pawn,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Ren_MaskEmotions)) AddPawnAbility(RWBYDefOf.Ren_MaskEmotions);
            }
            else if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Qrow)) // Qrow
            {
                aura = new Aura_Qrow
                {
                    pawn = Pawn,
                    maxEnergy = 1.2f,
                    currentEnergy = 1.2f
                };
            }
            else if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Raven)) // Raven
            {
                aura = new Aura_Raven
                {
                    pawn = Pawn,
                    maxEnergy = 1.2f,
                    currentEnergy = 1.2f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Raven_FormBond)) AddPawnAbility(RWBYDefOf.Raven_FormBond);
            }
            else if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Cinder)) // Cinder
            {
                aura = new Aura_Cinder
                {
                    pawn = Pawn,
                    maxEnergy = 1.2f,
                    currentEnergy = 1.2f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Cinder_ShootFireCrystal)) AddPawnAbility(RWBYDefOf.Cinder_ShootFireCrystal);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Cinder_SummonExplosives)) AddPawnAbility(RWBYDefOf.Cinder_SummonExplosives);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Cinder_CreateBlades)) AddPawnAbility(RWBYDefOf.Cinder_CreateBlades);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Cinder_CreateBow)) AddPawnAbility(RWBYDefOf.Cinder_CreateBow);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Cinder_CreateScimitar)) AddPawnAbility(RWBYDefOf.Cinder_CreateScimitar);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Cinder_CreateSpear)) AddPawnAbility(RWBYDefOf.Cinder_CreateSpear);
            }
            else if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Hazel)) // Hazel
            {
                aura = new Aura
                {
                    pawn = Pawn,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Hazel_IgnorePain)) AddPawnAbility(RWBYDefOf.Hazel_IgnorePain);
            }
            else if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Velvet)) // Velvet
            {
                aura = new Aura_Velvet
                {
                    pawn = Pawn,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
            }
            else if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Adam)) // Adam
            {
                aura = new Aura_Adam
                {
                    pawn = Pawn,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Adam_UnleashDamage)) AddPawnAbility(RWBYDefOf.Adam_UnleashDamage);
            }
            else if (Pawn.story.traits.HasTrait(RWBYDefOf.RWBY_Aura)) // Aura
            {
                aura = new Aura
                {
                    pawn = Pawn,
                    maxEnergy = 0.7f,
                    currentEnergy = 0.7f
                };
            }
            else if (LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().everyoneHasAura) // Optional
            {
                aura = new Aura
                {
                    pawn = Pawn,
                    maxEnergy = 0.3f,
                    currentEnergy = 0.3f
                };
            }
        }

        public bool TryUnlockSemblanceWith(SkillDef skillDef, bool forceUnlock = false)
        {
            if (forceUnlock)
            {
                return SemblanceUtility.UnlockSemblance(Pawn, hiddenSemblance, "LetterTextUnlockSemblanceGeneral");
            }
            if (SemblanceUtility.GetSemblancesForPassion(skillDef).Contains(hiddenSemblance))
            {
                return SemblanceUtility.UnlockSemblance(Pawn, hiddenSemblance, "LetterTextUnlock" + hiddenSemblance.defName.Replace("_",""));
            }
            return false;
        }

        public void GenerateHiddenSemblance()
        {
            if (Pawn.relations.RelatedPawns.Any(p => p.relations.Children.Contains(Pawn) && p.story.traits.HasTrait(RWBYDefOf.Semblance_Weiss)) || Pawn.relations.Children.Any(c => c.story.traits.HasTrait(RWBYDefOf.Semblance_Weiss)))
            {
                hiddenSemblance = RWBYDefOf.Semblance_Weiss;
                return;
            }
            List<TraitDef> traitDefs = new List<TraitDef>();
            foreach (SkillRecord skillRecord in Pawn.skills.skills)
            {
                if (skillRecord.passion == Passion.Minor)
                {
                    traitDefs.AddRange(SemblanceUtility.GetSemblancesForPassion(skillRecord.def));
                }
                else if (skillRecord.passion == Passion.Major)
                {
                    traitDefs.AddRange(SemblanceUtility.GetSemblancesForPassion(skillRecord.def));
                    traitDefs.AddRange(SemblanceUtility.GetSemblancesForPassion(skillRecord.def));
                }
            }
            traitDefs.RemoveAll(t => Pawn.WorkTagIsDisabled(t.requiredWorkTags));
            if (traitDefs.Count == 0)
            {
                hiddenSemblance = SemblanceUtility.semblanceList.FindAll(s => !Pawn.WorkTagIsDisabled(s.requiredWorkTags)).RandomElement(); // should never be empty, as there are Semblances without required workTags
            }
            else
            {
                List<TraitDef> allPossibleTraits = traitDefs.Distinct().ToList();
                Dictionary<TraitDef, int> keyValuePairs = new Dictionary<TraitDef, int>();
                foreach(TraitDef traitDef in traitDefs)
                {
                    if (keyValuePairs.ContainsKey(traitDef))
                    {
                        keyValuePairs[traitDef]++;
                    }
                    else
                    {
                        keyValuePairs.Add(traitDef, 1);
                    }
                }
                int highestCount = keyValuePairs.Values.ToList().OrderByDescending(i => i).First();
                List<TraitDef> mostMatchingTraits = new List<TraitDef>();
                foreach (KeyValuePair<TraitDef, int> keyValuePair in keyValuePairs)
                {
                    if (keyValuePair.Value == highestCount) mostMatchingTraits.Add(keyValuePair.Key);
                }
                hiddenSemblance = mostMatchingTraits.RandomElement();
            }
        }

        public override bool TryTransformPawn() // check if pawn can get an Aura
        {
            if (Pawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_AuraStolen)) return false;
            if (LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().everyoneHasAura) return true;
            if (Pawn.story.traits.HasTrait(RWBYDefOf.RWBY_Aura)) return true;
            if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Ruby)) return true;
            if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Yang)) return true;
            if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Weiss)) return true;
            if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Blake)) return true;
            if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Nora)) return true;
            if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Jaune)) return true;
            if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Pyrrha)) return true;
            if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Ren)) return true;
            if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Qrow)) return true;
            if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Raven)) return true;
            if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Cinder)) return true;
            if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Hazel)) return true;
            if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Velvet)) return true;
            if (Pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Adam)) return true;
            return false;
        }

        public void Notify_EatenPumkinPetes()
        {
            eatenPumkinPetesCounter += 1;
            if (eatenPumkinPetesCounter == 50)
            {
                eatenPumkinPetesCounter = 0;
                List<Thing> things = new List<Thing>();
                ThingWithComps thingWithComps = (ThingWithComps)ThingMaker.MakeThing(RWBYDefOf.Apparel_PumpkinPetes, GenStuff.RandomStuffFor(RWBYDefOf.Apparel_PumpkinPetes));
                thingWithComps.TryGetComp<CompQuality>().SetQuality((QualityCategory)Rand.RangeInclusive(0, 6), ArtGenerationContext.Colony);
                things.Add(thingWithComps);
                IntVec3 intVec = DropCellFinder.RandomDropSpot(Pawn.Map);
                DropPodUtility.DropThingsNear(intVec, Pawn.Map, things, 110, false, false, false);
                Find.LetterStack.ReceiveLetter("LetterLabelPumpkinPetePodCrash".Translate(), "LetterTextPumpkinPetePodCrash".Translate(), LetterDefOf.PositiveEvent, new TargetInfo(intVec, Pawn.Map, false), null, null);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            if (Initialized)
            {
                Scribe_Deep.Look(ref aura, false, parent.ThingID.ToString() + "Aura");
            }
            Scribe_Values.Look<int>(ref eatenPumkinPetesCounter, "eatenPumkinPetesCounter", 0, false);
            Scribe_Defs.Look<TraitDef>(ref hiddenSemblance, "hiddenSemblance");
            Scribe_Values.Look<int>(ref auraAutoUnlock, "auraAutoUnlock", Rand.Range(3600000, 7200000), false);
        }

        public int eatenPumkinPetesCounter = 0;
        public TraitDef hiddenSemblance = null;
        public int auraAutoUnlock = Rand.RangeInclusive(3600000, 7200000); // between 1 and 2 ingame years
    }
}
