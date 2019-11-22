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
                if (pawn.equipment.Primary == null && pawn.equipment.AllEquipmentListForReading.Find(e => e.def == RWBYDefOf.RWBY_Velvet_Camera_Box) is ThingWithComps thingWithComps)
                {
                    thingWithComps.TryGetComp<Weapon_TransformAbility>().Transform();
                }
                pawn.equipment.Primary.TryGetComp<Weapon_ProjectileAbility>().DoEffectOn(TargetA.Thing);
                pawn.needs.joy.GainJoy(0.07f, JoyKindDefOf.Social);
            };
            yield return takePhoto;
            
            job.count = 1;
            yield break;
        }
    }
}
