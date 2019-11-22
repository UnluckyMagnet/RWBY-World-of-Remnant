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
            MoteMaker.ThrowDustPuffThick(Position.ToVector3(), Map, 3, Def.color);
        }

        protected override void Impact(Thing hitThing)
        {
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_Smoke, null);
            moteThrown.Scale = Rand.Range(3.5f, 4.5f) * def.projectile.explosionRadius;
            moteThrown.rotationRate = Rand.Range(-30f, 30f);
            moteThrown.exactPosition = Position.ToVector3();
            moteThrown.instanceColor = Def.color;
            moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
            GenSpawn.Spawn(moteThrown, Position, Map, WipeMode.Vanish);
            MoteMaker.ThrowDustPuffThick(Position.ToVector3(), Map, 4, Def.color);
            base.Impact(hitThing);
        }
    }
}
