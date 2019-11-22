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
                    createdWeapon.TryGetComp<CompQuality>().SetQuality((QualityCategory)Rand.Range(0, 6), ArtGenerationContext.Colony);
                    GenSpawn.Spawn(createdWeapon, corpse.Position, corpse.Map);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_Smoke, null);
                moteThrown.Scale = Rand.Range(2.5f, 4.5f);
                moteThrown.rotationRate = Rand.Range(-30f, 30f);
                moteThrown.exactPosition = corpse.Position.ToVector3();
                moteThrown.instanceColor = color;
                moteThrown.SetVelocity((float)Rand.Range(0, 360), 0.2f);
                GenSpawn.Spawn(moteThrown, corpse.Position, corpse.Map, WipeMode.Vanish);
            }
            corpse.Destroy();
        }
    }
}
