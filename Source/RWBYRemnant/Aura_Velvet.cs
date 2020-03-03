using Verse;

namespace RWBYRemnant
{
    class Aura_Velvet : Aura
    {
        public override void TickRare()
        {
            if (pawn.Dead) return;
            if (pawn.equipment.Primary != null && pawn.equipment.Primary.TryGetComp<CompLightCopy>() != null)
            {
                Hediff hediffMimicMoves = new Hediff();
                hediffMimicMoves = HediffMaker.MakeHediff(RWBYDefOf.RWBY_VelvetMimicMoves, pawn);
                pawn.health.AddHediff(hediffMimicMoves);
            }
            else
            {
                pawn.health.hediffSet.hediffs.RemoveAll(h => h.def == RWBYDefOf.RWBY_VelvetMimicMoves);
            }
        }
    }
}
