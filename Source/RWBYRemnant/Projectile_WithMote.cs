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
            // method changed
            FleckMaker.ThrowDustPuffThick(Position.ToVector3(), Map, 2, Def.color);
            base.Tick();
        }

        // add method argument
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing);
        }
    }
}
