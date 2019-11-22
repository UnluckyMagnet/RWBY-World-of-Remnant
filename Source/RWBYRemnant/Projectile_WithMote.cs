using RimWorld;
using UnityEngine;
using Verse;

namespace RWBYRemnant
{
    public class ThingDef_MoteProjectile : ThingDef
    {
        public Color color;
    }

    public class Projectile_WithMote : Bullet
    {
        public ThingDef_MoteProjectile Def
        {
            get
            {
                return def as ThingDef_MoteProjectile;
            }
        }

        public override void Tick()
        {
            MoteMaker.ThrowDustPuffThick(Position.ToVector3(), Map, 2, Def.color);
            base.Tick();
        }

        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);
        }
    }
}
