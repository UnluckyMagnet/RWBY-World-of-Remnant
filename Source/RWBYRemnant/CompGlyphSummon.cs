using RimWorld;
using Verse;

namespace RWBYRemnant
{
    public class CompGlyphSummon : ThingComp
    {
        public CompProperties_GlyphSummon Props => (CompProperties_GlyphSummon)props;

        public int ticksToSummon;
        public PawnKindDef thingToSummon;

        public void SetData(int tmpTicksToSummon, PawnKindDef tmpThingToSummon)
        {
            ticksToSummon = tmpTicksToSummon;
            thingToSummon = tmpThingToSummon;
        }

        public override void CompTick()
        {
            ticksToSummon--;
            if (ticksToSummon < 1 && thingToSummon != null)
            {
                PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(thingToSummon, Faction.OfPlayer);
                Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
                pawn.Name = new NameSingle(thingToSummon.label, true);
                GenSpawn.Spawn(pawn, parent.Position, parent.Map);
                thingToSummon = null;
            }
            if (ticksToSummon < -60)
            {
                parent.Destroy();
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref ticksToSummon, "ticksToSummon", 1, false);
            Scribe_Values.Look<PawnKindDef>(ref thingToSummon, "thingToSummon", null, false);
        }
    }
}
