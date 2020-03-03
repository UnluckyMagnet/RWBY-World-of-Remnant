using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RWBYRemnant
{
    public class CompExplosiveSilent : CompExplosive
    {
        public override void PostDraw()
        {
        }

        public override void CompTick()
        {
            if (this.wickStarted)
            {
                this.wickTicksLeft--;
                if (this.wickTicksLeft <= 0)
                {
                    this.Detonate(this.parent.MapHeld);
                }
            }
        }

        public new void StartWick(Thing instigator = null)
        {
            if (this.wickStarted)
            {
                return;
            }
            if (this.ExplosiveRadius() <= 0f)
            {
                return;
            }
            this.wickStarted = true;
            this.wickTicksLeft = this.Props.wickTicks.RandomInRange;
            GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this.parent, this.Props.explosiveDamageType, null);
        }
    }
}
