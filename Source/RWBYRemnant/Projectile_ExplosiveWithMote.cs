using RimWorld;
using UnityEngine;
using Verse;

namespace RWBYRemnant
{
    public class ThingDef_ExplosiveMoteProjectile : ThingDef
    {
        public Color color;
    }

    public class Projectile_ExplosiveWithMote : Projectile_Explosive
    {
        public ThingDef_ExplosiveMoteProjectile Def
        {
            get
            {
                return def as ThingDef_ExplosiveMoteProjectile;
            }
        }

        public override void Tick()
        {
            base.Tick();
            FleckMaker.ThrowDustPuffThick(Position.ToVector3(), Map, 3, Def.color);
        }

        // add method argument
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            // ThingDefOf.Mote_Smoke changed FleckDefOf.Smoke
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(Position.ToVector3(), Map, FleckDefOf.Smoke);
            dataStatic.scale = Rand.Range(3.5f, 4.5f) * def.projectile.explosionRadius;
            dataStatic.rotationRate = Rand.Range(-30f, 30f);
            dataStatic.instanceColor = Def.color;
            dataStatic.velocityAngle = Rand.Range(30, 40);
            dataStatic.velocitySpeed = Rand.Range(0.5f, 0.7f);
            Map.flecks.CreateFleck(dataStatic);
            FleckMaker.ThrowDustPuffThick(Position.ToVector3(), Map, 4, Def.color);
            base.Impact(hitThing);
        }
    }
}
