using RimWorld;
using UnityEngine;
using Verse;

namespace RWBYRemnant
{
    public class ThingDef_SilverEyes : ThingDef
    {
        
    }

    public class Projectile_SilverEyes : Bullet
    {
        public ThingDef_SilverEyes Def
        {
            get
            {
                return def as ThingDef_SilverEyes;
            }
        }

        protected override void Impact(Thing hitThing)
        {
            if (hitThing is Pawn targetPawn && launcher is Pawn CasterPawn)
            {
                float damageToDeal = 50f;
                damageToDeal = damageToDeal * CasterPawn.health.hediffSet.hediffs.FindAll(h => h.def == RWBYDefOf.RWBY_SilverEyes).Count;
                DamageInfo dinfo1 = new DamageInfo(DamageDefOf.Burn, damageToDeal, 0f, -1f, CasterPawn, null, CasterPawn.def, DamageInfo.SourceCategory.ThingOrUnknown, targetPawn);
                dinfo1.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                Vector3 direction1 = (targetPawn.Position - CasterPawn.Position).ToVector3();
                dinfo1.SetAngle(direction1);
                targetPawn.stances.stunner.StunFor(GenTicks.SecondsToTicks(5f), CasterPawn);
                targetPawn.TakeDamage(dinfo1);
            }
            this.Destroy(DestroyMode.Vanish);
        }       
    }
}
