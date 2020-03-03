using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace RWBYRemnant
{
    class JoyGiver_TakePhotos : JoyGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            if ((!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().everyoneMakesPhotosForJoy && !pawn.story.traits.HasTrait(RWBYDefOf.Semblance_Velvet)) || !pawn.equipment.AllEquipmentListForReading.Any(e => e.def == RWBYDefOf.RWBY_Anesidora_Camera || e.def == RWBYDefOf.RWBY_Anesidora_Box))
            {
                return null;
            }
            if (pawn.equipment.Primary != null && pawn.equipment.Primary.def != RWBYDefOf.RWBY_Anesidora_Camera)
            {
                return null;
            }

            List<Pawn> potentialPawnTargets = new List<Pawn>();
            potentialPawnTargets.AddRange(pawn.Map.mapPawns.AllPawnsSpawned.FindAll(p => p.RaceProps.Humanlike && pawn.CanReach(p, PathEndMode.InteractionCell, Danger.None) && p != pawn));
            List<Thing> potentialWeaponTargets = new List<Thing>();
            potentialWeaponTargets.AddRange(pawn.Map.GetDirectlyHeldThings().ToList().FindAll(t => t.def.IsWeapon && t.def.equipmentType == EquipmentType.Primary && pawn.CanReach(t, PathEndMode.InteractionCell, Danger.None)));
            if (potentialPawnTargets.Count == 0 && potentialWeaponTargets.Count == 0)
            {
                return null;
            }

            Job job;
            if (Rand.Chance((float)potentialPawnTargets.Count / ((float)potentialPawnTargets.Count + (float)potentialWeaponTargets.Count)))
            {
                job = new Job(this.def.jobDef, potentialPawnTargets.RandomElement());
                job.locomotionUrgency = LocomotionUrgency.Jog;
            }
            else
            {
                job = new Job(this.def.jobDef, potentialWeaponTargets.RandomElement());
                job.locomotionUrgency = LocomotionUrgency.Jog;
            }
            return job;
        }
    }
}
