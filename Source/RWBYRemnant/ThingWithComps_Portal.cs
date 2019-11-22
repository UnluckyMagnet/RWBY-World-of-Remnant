using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RWBYRemnant
{
    class ThingWithComps_Portal : ThingWithComps
    {
        public override void Tick()
        {
            base.Tick();
            timeToLive--;
            if (timeToLive <= 0) this.Destroy();
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(selPawn)) yield return floatMenuOption;
            string label = "Use Portal";
            yield return new FloatMenuOption(label, delegate()
            {
                Job job = new Job(RWBYDefOf.GoThroughPortal, this);
                if (job != null) selPawn.jobs.TryTakeOrderedJob(job);
            }, MenuOptionPriority.Low, null, null, 0f, null, null);
            yield break;
        }

        public void Teleport(Pawn p)
        {
            bool drafted = p.Drafted;
            p.DeSpawn();
            GenSpawn.Spawn(p, targetPosition, targetMap);
            p.drafter.Drafted = drafted;
        }

        public void RegisterConnectedPortal(Thing t)
        {
            targetMap = t.Map;
            targetPosition = t.Position;
        }

        private Map targetMap;
        private IntVec3 targetPosition;
        private int timeToLive = GenTicks.SecondsToTicks(30);

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Map>(ref targetMap, "targetMap", null, false);
            Scribe_Values.Look<IntVec3>(ref targetPosition, "targetPosition", new IntVec3(), false);
            Scribe_Values.Look<int>(ref timeToLive, "timeToLive", GenTicks.SecondsToTicks(30), false);
        }
    }
}
