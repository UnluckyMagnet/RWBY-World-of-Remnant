using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RWBYRemnant
{
    public class CompLightCopy : CompUseEffect
    {
        public CompEquippable GetEquippable => parent.GetComp<CompEquippable>();

        public Pawn GetPawn => GetEquippable.verbTracker.PrimaryVerb.CasterPawn;

        public void CheckForOwner()
        {
            if (GetPawn == null || !GetPawn.equipment.AllEquipmentListForReading.Contains(parent))
            {
                parent.Destroy();
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            CheckForOwner();
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            CheckForOwner();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            CheckForOwner();
            yield break;
        }
    }
}
