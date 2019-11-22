using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RWBYRemnant
{
    public class JobDriverPickUp : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(TargetIndex.A), job);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            Toil reserveTargetA = Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
            yield return reserveTargetA;
            Toil toilGoto = null;
            toilGoto = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnSomeonePhysicallyInteracting(TargetIndex.A).FailOn(delegate ()
            {
                Pawn actor = toilGoto.actor;
                Job curJob = actor.jobs.curJob;
                if (curJob.haulMode == HaulMode.ToCellStorage)
                {
                    Thing thing = curJob.GetTarget(TargetIndex.A).Thing;
                    IntVec3 cell = actor.jobs.curJob.GetTarget(TargetIndex.B).Cell;
                    if (!cell.IsValidStorageFor(this.Map, thing))
                    {
                        return true;
                    }
                }
                return false;
            });
            yield return toilGoto;
            job.count = Mathf.Min(10, TargetA.Thing.stackCount);
            yield return Toils_Haul.TakeToInventory(TargetIndex.A, job.count);
            yield break;
        }
    }
}
