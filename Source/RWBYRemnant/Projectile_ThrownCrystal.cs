using RimWorld;
using Verse;

namespace RWBYRemnant
{
    public class ThingDef_CrystalBullet : ThingDef
    {
        public ThingDef spawnOnImpact;
    }

    public class Projectile_CrystalBullet : Bullet
    {
        public ThingDef_CrystalBullet Def
        {
            get
            {
                return def as ThingDef_CrystalBullet;
            }
        }

        protected override void Impact(Thing hitThing)
        {
            GenSpawn.Spawn(Def.spawnOnImpact, Position, Map);
            Destroy(DestroyMode.Vanish);
        }
    }
}
