using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RWBYRemnant
{
    public class WorkGiver_StealAura : WorkGiver
    {
        public Job JobOnThing(Pawn actorPawn, Pawn targetPawn, Thing sourceThing, bool forced = false)
        {
            Job job = new Job(RWBYDefOf.RWBY_StealAura, targetPawn, sourceThing);
            return job;
        }
    }
}
