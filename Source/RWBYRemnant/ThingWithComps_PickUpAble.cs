using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RWBYRemnant
{
    class ThingWithComps_PickUpAble : ThingWithComps
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(selPawn)) yield return floatMenuOption;
            string label = "Pick up x10";
            if (stackCount <= 10)
            {
                label = "Pick up x" + stackCount.ToString() + " (all)";
            }
            yield return new FloatMenuOption(label, delegate()
            {
                WorkGiver_PickUp pickUpWorker = new WorkGiver_PickUp();
                Job job = pickUpWorker.JobOnThing(selPawn, this, true);
                if (job != null) selPawn.jobs.TryTakeOrderedJob(job);
            }, MenuOptionPriority.Low, null, null, 0f, null, null);
            yield break;
        }
    }
}
