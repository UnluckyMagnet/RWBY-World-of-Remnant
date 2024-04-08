﻿using RimWorld;
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

        // add method argument
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            if (launcher is Pawn launcherPawn && launcherPawn != null)
            {
                launcherPawn.Position = Position;
                launcherPawn.Notify_Teleported(true, false);
                launcherPawn.equipment.Primary.TryGetComp<CompWeaponTransform>().Transform();
            }
            base.Impact(hitThing);
        }
    }
}
