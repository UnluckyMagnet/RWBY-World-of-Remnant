using RimWorld;
using Verse;
using Verse.AI;

namespace RWBYRemnant
{
    public class Pawn_SummonedGrimm : Pawn
    {
        public override void PostMake()
        {
            base.PostMake();
            ticksToLive = GenTicks.SecondsToTicks(60f);
        }

        public override void Tick()
        {
            base.Tick();
            ticksToLive--;
            if (ticksToLive < 1 || Downed)
            {
                this.Kill(null);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref ticksToLive, "ticksToLive", 1, false);
        }

        public int ticksToLive;
    }
}
