using RimWorld;
using Verse;

namespace RWBYRemnant
{
    public class ThingDef_WiltBullet : ThingDef
    {
        public bool rushAfterOnMiss;
    }

    public class Projectile_WiltBullet : Bullet
    {
        public ThingDef_WiltBullet Def
        {
            get
            {
                return def as ThingDef_WiltBullet;
            }
        }

        protected override void Impact(Thing hitThing)
        {
            if (launcher is Pawn launcherPawn && launcherPawn != null)
            {
                launcherPawn.Position = Position;
                launcherPawn.Notify_Teleported(true, false);
                launcherPawn.equipment.Primary.TryGetComp<Weapon_TransformAbility>().Transform();
            }
            base.Impact(hitThing);
        }
    }
}
