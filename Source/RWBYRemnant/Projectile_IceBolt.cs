using RimWorld;
using Verse;

namespace RWBYRemnant
{
    public class Myrtenaster_IceBolt : ThingDef
    {
    }

    public class Projectile_IceBolt : Bullet
    {
        public Myrtenaster_IceBolt Def
        {
            get
            {
                return def as Myrtenaster_IceBolt;
            }
        }

        public override void Tick()
        {
            base.Tick();
            if (!Position.ToVector3().ShouldSpawnMotesAt(Map) || Map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_Smoke, null);
            moteThrown.Scale = Rand.Range(1.5f, 2.5f) * 1;
            moteThrown.rotationRate = Rand.Range(-30f, 30f);
            moteThrown.exactPosition = Position.ToVector3();
            moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
            GenSpawn.Spawn(moteThrown, Position, Map, WipeMode.Vanish);
        }

        protected override void Impact(Thing hitThing)
        {
            SnowUtility.AddSnowRadial(Position, Map, 2f, 1f);
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_Smoke, null);
            moteThrown.Scale = Rand.Range(1.5f, 2.5f) * 2;
            moteThrown.rotationRate = Rand.Range(-30f, 30f);
            moteThrown.exactPosition = Position.ToVector3();
            moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
            GenSpawn.Spawn(moteThrown, Position, Map, WipeMode.Vanish);
            base.Impact(hitThing);
        }
    }
}
