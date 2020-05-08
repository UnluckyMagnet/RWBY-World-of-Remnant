using Verse;
using RimWorld;
using UnityEngine;

namespace RWBYRemnant
{
    public class Aura_Ruby : Aura
    {
        public override void Tick()
        {
            if (pawn.IsFighting() && pawn.equipment.AllEquipmentListForReading.Any(t => t.def == RWBYDefOf.RWBY_Crescent_Rose_Rifle || t.def == RWBYDefOf.RWBY_Crescent_Rose_Scythe))
            {
                Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(RWBYDefOf.RWBY_RubyUsedCrescentRose);
                pawn.needs.mood.thoughts.memories.TryGainMemory(thought_Memory);
            }
            base.Tick();
        }

        public override Color GetColor()
        {
            return color;
        }

        public Color color = new Color(1.0f, 0f, 0f);
    }
}
