using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RWBYRemnant
{
    public class JobDriverGoThroughPortal : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
            Toil teleportAction = new Toil();
            teleportAction.initAction = delegate ()
            {
                ThingWithComps_Portal portal = (ThingWithComps_Portal)TargetA;
                if (portal != null)
                {
                    portal.Teleport(pawn);
                }
            };            
            yield return teleportAction;
            yield break;
        }
    }
}
