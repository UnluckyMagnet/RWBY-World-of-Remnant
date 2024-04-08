using RimWorld;
using UnityEngine;
using Verse;

namespace RWBYRemnant
{
    public class Grimm_DeathWorker : DeathActionWorker
    {
        public override void PawnDied(Corpse corpse)
        {
            Color color = new Color();

            if (corpse.InnerPawn.Faction == Faction.OfPlayer)
            {
                color = new Color(1, 1, 1); // White Smoke
            }
            else
            {
                color = new Color(0, 0, 0); // Black Smoke
                if (Rand.Chance(corpse.InnerPawn.RaceProps.AnyPawnKind.combatPower / 100000))
                {
                    ThingWithComps createdWeapon = (ThingWithComps)ThingMaker.MakeThing(RWBYDefOf.RWBY_Grimm_Glove);
                    createdWeapon.TryGetComp<CompQuality>().SetQuality((QualityCategory)Rand.RangeInclusive(0, 6), ArtGenerationContext.Colony);
                    GenSpawn.Spawn(createdWeapon, corpse.Position, corpse.Map);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                // ThingDefOf.Mote_Smoke changed FleckDefOf.Smoke
                FleckCreationData dataStatic = FleckMaker.GetDataStatic(corpse.Position.ToVector3(), corpse.Map, FleckDefOf.Smoke);
                dataStatic.scale = Rand.Range(2.5f, 4.5f);
                dataStatic.rotationRate = Rand.Range(-30f, 30f);
                dataStatic.instanceColor = color;
                dataStatic.velocityAngle = Rand.Range(0, 360);
                dataStatic.velocitySpeed = 0.2f;
                corpse.Map.flecks.CreateFleck(dataStatic);
            }
            corpse.Destroy();
        }
    }
}
