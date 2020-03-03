using System.Collections.Generic;
using Verse.AI;

namespace RWBYRemnant
{
    public class JobDriverWeaponShootProjectileAbility : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil addVerb = new Toil();
            addVerb.initAction = delegate ()
            {
                pawn.verbTracker.AllVerbs.Add(pawn.CurJob.verbToUse);
            };
            yield return addVerb;

            yield return Toils_Combat.CastVerb(TargetIndex.A);

            Toil removeVerb = new Toil();
            removeVerb.initAction = delegate ()
            {
                pawn.verbTracker.AllVerbs.Remove(pawn.CurJob.verbToUse);
            };
            yield return removeVerb;
            yield break;
        }
    }
}