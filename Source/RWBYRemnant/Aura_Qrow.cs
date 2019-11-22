using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;
using Verse.AI;
using System.Linq;
using System.Reflection;

namespace RWBYRemnant
{
    public class Aura_Qrow : Aura
    {
        public override void Tick()
        {
            base.Tick();
            if (pawn.Map != null && currentEnergy > 0f && Find.TickManager.TicksGame % GenTicks.SecondsToTicks(15) == 0)
            {
                List<Pawn> pawns = pawn.Map.mapPawns.AllPawnsSpawned.FindAll(p => p.RaceProps.Humanlike && p.Position.DistanceTo(pawn.Position) < misfortuneRange && p != pawn);
                if (pawns.NullOrEmpty())
                {
                    return;
                }
                Pawn affectedPawn = pawns.RandomElement();

                if (affectedPawn.CurJobDef == JobDefOf.FinishFrame)
                {
                    Frame frame = (Frame)affectedPawn.CurJob.GetTarget(TargetIndex.A).Thing;
                    frame.FailConstruction(affectedPawn);
                    affectedPawn.jobs.curDriver.ReadyForNextToil();
                    MoteMaker.ThrowText(affectedPawn.DrawPos, affectedPawn.Map, "TextMote_ConstructionFail".Translate(), GetColor(), 3.65f);
                    return;
                }
                if (affectedPawn.CurJobDef == JobDefOf.Harvest)
                {
                    Plant plant = (Plant)affectedPawn.CurJob.GetTarget(TargetIndex.A).Thing;
                    Vector3 loc = (affectedPawn.DrawPos + plant.DrawPos) / 2f;
                    MoteMaker.ThrowText(loc, affectedPawn.Map, "TextMote_HarvestFailed".Translate(), GetColor(), 3.65f);
                    plant.def.plant.soundHarvestFinish.PlayOneShot(affectedPawn);
                    plant.PlantCollected();
                    FieldInfo fieldInfo = typeof(JobDriver_PlantWork).GetField("workDone", BindingFlags.NonPublic | BindingFlags.Instance);
                    fieldInfo.SetValue((JobDriver_PlantWork)affectedPawn.jobs.curDriver, 0f);
                    affectedPawn.jobs.curDriver.ReadyForNextToil();
                    return;
                }
                if (affectedPawn.CurJobDef == JobDefOf.Ingest && affectedPawn.RaceProps.body.AllParts.Any(b => b.def == BodyPartDefOf.Jaw))
                {
                    DamageInfo damageInfo = new DamageInfo(DamageDefOf.Bite, 1f, 0, -1, affectedPawn);
                    damageInfo.SetAmount(1f);
                    damageInfo.SetHitPart(affectedPawn.RaceProps.body.AllParts.Find(b => b.def == BodyPartDefOf.Jaw));
                    affectedPawn.TakeDamage(damageInfo);
                    MoteMaker.ThrowText(affectedPawn.DrawPos, affectedPawn.Map, "TextMote_Crow_BitLip".Translate(), GetColor(), 3.65f);
                    return;
                }
                if (affectedPawn.CurJobDef == JobDefOf.Wait_Wander && affectedPawn.RaceProps.body.AllParts.Any(b => b.def.defName.ToUpper().Contains("TOE")))
                {
                    DamageInfo damageInfo = new DamageInfo(DamageDefOf.Blunt, 1f, 0, -1, affectedPawn);
                    damageInfo.SetAmount(1f);
                    damageInfo.SetHitPart(affectedPawn.RaceProps.body.AllParts.FindAll(b => b.def.defName.ToUpper().Contains("TOE")).RandomElement());
                    affectedPawn.TakeDamage(damageInfo);
                    MoteMaker.ThrowText(affectedPawn.DrawPos, affectedPawn.Map, "TextMote_Crow_StubbedToe".Translate(), GetColor(), 3.65f);
                    return;
                }
                if (affectedPawn.IsFighting() && affectedPawn.equipment.Primary != null && affectedPawn.HostileTo(pawn))
                {
                    affectedPawn.equipment.TryDropEquipment(affectedPawn.equipment.Primary, out ThingWithComps resultingEq, affectedPawn.Position, false);
                    MoteMaker.ThrowText(affectedPawn.DrawPos, affectedPawn.Map, "TextMote_Crow_WeaponSlipped".Translate(), GetColor(), 3.65f);
                    return;
                }
            }
        }

        public override bool TryAbsorbDamage(DamageInfo dinfo)
        {
            if (currentEnergy > 0f && dinfo.Instigator is Pawn instigatorPawn && Rand.Chance(0.5f))
            {
                if (dinfo.Def.isRanged)
                {
                    switch (Rand.Range(1, 4))
                    {
                        case 1:
                            RWBYDefOf.Ricochet1.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
                            break;

                        case 2:
                            RWBYDefOf.Ricochet2.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
                            break;

                        case 3:
                            RWBYDefOf.Ricochet3.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
                            break;

                        case 4:
                            RWBYDefOf.Ricochet4.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
                            break;
                    }
                    MoteMaker.ThrowText(instigatorPawn.DrawPos, instigatorPawn.Map, "TextMote_Crow_MissedRanged".Translate(), GetColor(), 3.65f);
                    return true;
                }
                else
                {
                    MoteMaker.ThrowText(instigatorPawn.DrawPos, instigatorPawn.Map, "TextMote_Crow_MissedMelee".Translate(), GetColor(), 3.65f);
                    SoundDefOf.Pawn_Melee_Punch_Miss.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
                }
            }
            return base.TryAbsorbDamage(dinfo);
        }

        public override Color GetColor()
        {
            return new Color(1.0f, 0f, 0f);
        }

        public float misfortuneRange = 10f;
    }
}
