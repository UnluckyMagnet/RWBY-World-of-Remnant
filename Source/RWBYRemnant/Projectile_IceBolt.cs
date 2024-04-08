using RimWorld;
using Verse;
using Verse.Noise;

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
            // ThingDefOf.Mote_Smoke changed FleckDefOf.Smoke
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(Position.ToVector3(), Map, FleckDefOf.Smoke);
            dataStatic.scale = Rand.Range(1.5f, 2.5f) * 1;
            dataStatic.rotationRate = Rand.Range(-30f, 30f);
            dataStatic.velocityAngle = Rand.Range(30, 40);
            dataStatic.velocitySpeed = Rand.Range(0.5f, 0.7f);
            Map.flecks.CreateFleck(dataStatic);
        }

        // add method argument
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            SnowUtility.AddSnowRadial(Position, Map, 2f, 1f);
            // ThingDefOf.Mote_Smoke changed FleckDefOf.Smoke
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(Position.ToVector3(), Map, FleckDefOf.Smoke);
            dataStatic.scale = Rand.Range(1.5f, 2.5f) * 1;
            dataStatic.rotationRate = Rand.Range(-30f, 30f);
            dataStatic.velocityAngle = Rand.Range(30, 40);
            dataStatic.velocitySpeed = Rand.Range(0.5f, 0.7f);
            Map.flecks.CreateFleck(dataStatic);
            base.Impact(hitThing);
        }
    }
}
