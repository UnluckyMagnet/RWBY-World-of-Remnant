using RimWorld;
using Verse;
using Verse.Sound;

namespace RWBYRemnant
{
    public class ThingDef_CameraBullet : ThingDef
    {

    }

    public class Projectile_CameraBullet : Bullet
    {
        public ThingDef_CameraBullet Def
        {
            get
            {
                return def as ThingDef_CameraBullet;
            }
        }

        // add method argument
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Destroy(DestroyMode.Vanish);
            if (intendedTarget.Thing == null) return;
            if (((Pawn)launcher).equipment.AllEquipmentListForReading.Find(x => x.def.Equals(RWBYDefOf.RWBY_Anesidora_Camera)).TryGetComp<CompTakePhoto>() == null) return;
            string photoOf = "";

            if (intendedTarget.Thing.GetType().Equals(typeof(Pawn))) // took photo of pawn
            {
                Pawn targetPawn = (Pawn)intendedTarget;
                if (targetPawn != null && targetPawn.RaceProps.Humanlike)
                {
                    Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(RWBYDefOf.RWBY_PictureTaken);
                    targetPawn.needs.mood.thoughts.memories.TryGainMemory(thought_Memory);
                }                
                if (targetPawn != null && (targetPawn.Drafted || targetPawn.IsFighting() || targetPawn.RaceProps.IsMechanoid || ((Pawn)launcher).CurJobDef == RWBYDefOf.RWBY_TakePhotos) && targetPawn.equipment.Primary != null)
                {
                    photoOf = targetPawn.equipment.Primary.def.defName;
                    ((Pawn)launcher).equipment.AllEquipmentListForReading.Find(x => x.def.Equals(RWBYDefOf.RWBY_Anesidora_Camera)).TryGetComp<CompTakePhoto>().ListOfDifferentPhotos.Add(photoOf);
                }
            }

            if (intendedTarget.Thing.GetType().Equals(typeof(ThingWithComps))) // took photo of thing
            {
                ThingWithComps targetThing = (ThingWithComps)intendedTarget;
                if (targetThing.def.equipmentType.Equals(EquipmentType.Primary))
                {
                    photoOf = targetThing.def.defName;
                    ((Pawn)launcher).equipment.AllEquipmentListForReading.Find(x => x.def.Equals(RWBYDefOf.RWBY_Anesidora_Camera)).TryGetComp<CompTakePhoto>().ListOfDifferentPhotos.Add(photoOf);
                }
            }

        }
    }
}
