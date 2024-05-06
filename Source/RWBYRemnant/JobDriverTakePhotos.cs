using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RWBYRemnant
{
    class JobDriverTakePhotos : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(TargetIndex.A), job);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            if (TargetA.Thing is Pawn targetPawn)
            {
                Toil toilGoto = null;
                toilGoto = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
                toilGoto.tickAction = delegate ()
                {
                    JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.EndJob, 0.3f, null);
                };
                yield return toilGoto;
            }
            else
            {
                Toil reserveTargetA = Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
                yield return reserveTargetA;

                Toil toilGoto = null;
                toilGoto = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
                toilGoto.tickAction = delegate ()
                {
                    JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.EndJob, 0.3f, null);
                };
                yield return toilGoto;
            }

            Toil takePhoto = new Toil();
            takePhoto.initAction = delegate ()
            {
                if (pawn.equipment.Primary == null && pawn.equipment.AllEquipmentListForReading.Find(e => e.def == RWBYDefOf.RWBY_Anesidora_Box) is ThingWithComps thingWithComps)
                {
                    thingWithComps.TryGetComp<CompWeaponTransform>().Transform();
                }
                pawn.needs.joy.GainJoy(0.07f, JoyKindDefOf.Social);

                Verb_ShootWeaponAbility verb_ShootWeaponAbility = new Verb_ShootWeaponAbility
                {
                    verbTracker = new VerbTracker(pawn),
                    caster = pawn,
                    verbProps = new VerbProperties
                    {
                        accuracyTouch = 1f,
                        accuracyShort = 1f,
                        accuracyMedium = 1f,
                        accuracyLong = 1f,
                        verbClass = typeof(Verb_ShootWeaponAbility),
                        hasStandardCommand = true,
                        defaultProjectile = RWBYDefOf.Bullet_Velvet_Camera,
                        warmupTime = 2f,
                        range = 10f,
                        soundCast = RWBYDefOf.Shot_Velvet_Camera,
                        burstShotCount = 1,
                        ticksBetweenBurstShots = 1,
                        muzzleFlashScale = 0,
                    },
                    cannotMiss = true,
                    ammunition = null
                };
                verb_ShootWeaponAbility.verbProps.targetParams = TargetingParameters.ForAttackAny();
                verb_ShootWeaponAbility.TryStartCastOn(TargetA.Thing);
            };
            yield return takePhoto;
            
            job.count = 1;
            yield break;
        }
    }
}
