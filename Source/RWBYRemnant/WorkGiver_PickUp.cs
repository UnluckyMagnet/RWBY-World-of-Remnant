using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RWBYRemnant
{
    public class WorkGiver_PickUp : WorkGiver_HaulGeneral
    {
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!forced && MassUtility.WillBeOverEncumberedAfterPickingUp(pawn, t, Mathf.Min(10, t.stackCount))) return null;
            if (!GoodThingToHaul(t, pawn)) return null;

            Job job = new Job(RWBYDefOf.RWBY_PickUp, t);
            return job;
        }

        public static bool GoodThingToHaul(Thing t, Pawn pawn)
        {
            return t.Spawned && !t.IsForbidden(pawn) && pawn.CanReserveAndReach(t, PathEndMode.Touch, Danger.Some);
        }
    }
}
