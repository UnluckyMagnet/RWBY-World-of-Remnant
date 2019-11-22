using RimWorld;
using System.Collections.Generic;
using Verse;
using AbilityUser;
using UnityEngine;

namespace RWBYRemnant
{
    public class CompAbilityUserAura : CompAbilityUser
    {
        public Aura aura;

        public override void CompTick()
        {
            base.CompTick();
            if (Pawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_AuraStolen)) return;
            if (IsInitialized)
            {
                aura.Tick();
            }
            else
            {
                if (auraAutoUnlock >= 0) auraAutoUnlock--;
                if (auraAutoUnlock == 0)
                {
                    SemblanceUtility.UnlockAura(AbilityUser, "LetterTextUnlockAuraAuto");
                }
            }
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            if (Pawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_AuraStolen))
            {
                IsInitialized = false;
                aura = null;
                AbilityData.AllPowers.Clear();
                return;
            }
            if (IsInitialized)
            {
                aura.TickRare();
                if (!AbilityUser.story.traits.allTraits.Any(t => t.def == RWBYDefOf.RWBY_Aura || SemblanceUtility.semblanceList.Contains(t.def)) && !LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().everyoneHasAura)
                {
                    IsInitialized = false;
                    aura = null;
                    AbilityData.AllPowers.Clear();
                }
                //if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Weiss))
                //{
                //    if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_SummonLancer)) AddPawnAbility(RWBYDefOf.Weiss_SummonLancer);
                //}
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
            if (IsInitialized)
            {
                yield return new GizmoAuraStatus
                {
                    aura = aura,
                    label = "AuraGizmoLabel".Translate(AbilityUser.Name.ToStringShort),
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
            if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Ruby)) // Ruby
            {
                aura = new Aura_Ruby
                {
                    pawn = AbilityUser,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Ruby_BurstIntoRosePetals)) AddPawnAbility(RWBYDefOf.Ruby_BurstIntoRosePetals);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Ruby_CarryPawn)) AddPawnAbility(RWBYDefOf.Ruby_CarryPawn);
            }
            else if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Yang)) // Yang
            {
                aura = new Aura_Yang
                {
                    pawn = AbilityUser,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
                AddPawnAbility(RWBYDefOf.Yang_ReturnDamage);
            }
            else if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Weiss)) // Weiss
            {
                aura = new Aura_Weiss
                {
                    pawn = AbilityUser,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_TimeDilationGlyph_Summon)) AddPawnAbility(RWBYDefOf.Weiss_TimeDilationGlyph_Summon);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_SummonBoar)) AddPawnAbility(RWBYDefOf.Weiss_SummonBoar);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_SummonArmaGigas)) AddPawnAbility(RWBYDefOf.Weiss_SummonArmaGigas);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_SummonArmaGigasSword)) AddPawnAbility(RWBYDefOf.Weiss_SummonArmaGigasSword);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Weiss_SummonLancer)) AddPawnAbility(RWBYDefOf.Weiss_SummonLancer);
            }
            else if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Blake)) // Blake
            {
                aura = new Aura_Blake
                {
                    pawn = AbilityUser,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Blake_UseStoneClone)) AddPawnAbility(RWBYDefOf.Blake_UseStoneClone);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Blake_UseIceClone)) AddPawnAbility(RWBYDefOf.Blake_UseIceClone);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Blake_UseFireClone)) AddPawnAbility(RWBYDefOf.Blake_UseFireClone);
            }
            else if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Nora)) // Nora
            {
                aura = new Aura_Nora
                {
                    pawn = AbilityUser,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
            }
            else if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Jaune)) // Jaune
            {
                aura = new Aura_Jaune
                {
                    pawn = AbilityUser,
                    maxEnergy = 1.5f,
                    currentEnergy = 1.5f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Jaune_AmplifyAura)) AddPawnAbility(RWBYDefOf.Jaune_AmplifyAura);
            }
            else if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Pyrrha)) // Pyrrha
            {
                aura = new Aura_Pyrrha
                {
                    pawn = AbilityUser,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Pyrrha_UnlockAura)) AddPawnAbility(RWBYDefOf.Pyrrha_UnlockAura);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Pyrrha_UseMagnetism)) AddPawnAbility(RWBYDefOf.Pyrrha_UseMagnetism);
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Pyrrha_MagneticPulse)) AddPawnAbility(RWBYDefOf.Pyrrha_MagneticPulse);
            }
            else if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Ren)) // Ren
            {
                aura = new Aura_Ren
                {
                    pawn = AbilityUser,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Ren_MaskEmotions)) AddPawnAbility(RWBYDefOf.Ren_MaskEmotions);
            }
            else if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Qrow)) // Qrow
            {
                aura = new Aura_Qrow
                {
                    pawn = AbilityUser,
                    maxEnergy = 1.2f,
                    currentEnergy = 1.2f
                };
            }
            else if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Raven)) // Raven
            {
                aura = new Aura_Raven
                {
                    pawn = AbilityUser,
                    maxEnergy = 1.2f,
                    currentEnergy = 1.2f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Raven_FormBond)) AddPawnAbility(RWBYDefOf.Raven_FormBond);
            }
            else if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Cinder)) // Cinder
            {
                aura = new Aura_Cinder
                {
                    pawn = AbilityUser,
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
            else if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Hazel)) // Hazel
            {
                aura = new Aura
                {
                    pawn = AbilityUser,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
                if (!AbilityData.AllPowers.Any(p => p.Def == RWBYDefOf.Hazel_IgnorePain)) AddPawnAbility(RWBYDefOf.Hazel_IgnorePain);
            }
            else if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Velvet)) // Velvet
            {
                aura = new Aura_Velvet
                {
                    pawn = AbilityUser,
                    maxEnergy = 1f,
                    currentEnergy = 1f
                };
            }
            else if (AbilityUser.story.traits.HasTrait(RWBYDefOf.RWBY_Aura)) // Aura
            {
                aura = new Aura
                {
                    pawn = AbilityUser,
                    maxEnergy = 0.7f,
                    currentEnergy = 0.7f
                };
            }
            else if (LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().everyoneHasAura) // Optional
            {
                aura = new Aura
                {
                    pawn = AbilityUser,
                    maxEnergy = 0.3f,
                    currentEnergy = 0.3f
                };
            }
        }

        public override bool TryTransformPawn() // check if pawn can get an Aura
        {
            if (Pawn.health.hediffSet.HasHediff(RWBYDefOf.RWBY_AuraStolen)) return false;
            if (LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().everyoneHasAura) return true;
            if (AbilityUser.story.traits.HasTrait(RWBYDefOf.RWBY_Aura)) return true;
            if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Ruby)) return true;
            if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Yang)) return true;
            if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Weiss)) return true;
            if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Blake)) return true;
            if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Nora)) return true;
            if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Jaune)) return true;
            if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Pyrrha)) return true;
            if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Ren)) return true;
            if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Qrow)) return true;
            if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Raven)) return true;
            if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Cinder)) return true;
            if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Hazel)) return true;
            if (AbilityUser.story.traits.HasTrait(RWBYDefOf.Semblance_Velvet)) return true;
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
                thingWithComps.TryGetComp<CompQuality>().SetQuality((QualityCategory)Rand.Range(0, 6), ArtGenerationContext.Colony);
                things.Add(thingWithComps);
                IntVec3 intVec = DropCellFinder.RandomDropSpot(AbilityUser.Map);
                DropPodUtility.DropThingsNear(intVec, AbilityUser.Map, things, 110, false, false, false);
                Find.LetterStack.ReceiveLetter("LetterLabelPumpkinPetePodCrash".Translate(), "LetterTextPumpkinPetePodCrash".Translate(), LetterDefOf.PositiveEvent, new TargetInfo(intVec, AbilityUser.Map, false), null, null);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            if (IsInitialized)
            {
                Scribe_Deep.Look(ref aura, false, parent.ThingID.ToString() + "Aura");
            }
            Scribe_Values.Look<int>(ref eatenPumkinPetesCounter, "eatenPumkinPetesCounter", 0, false);
            Scribe_Values.Look<int>(ref auraAutoUnlock, "auraAutoUnlock", Rand.Range(3600000, 7200000), false);
        }

        public int eatenPumkinPetesCounter = 0;
        public int auraAutoUnlock = Rand.Range(3600000, 7200000); // between 1 and 2 ingame years
    }
}
