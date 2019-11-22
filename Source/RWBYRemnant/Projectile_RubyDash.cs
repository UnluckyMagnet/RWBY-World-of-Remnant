using RimWorld;
using UnityEngine;
using Verse;

namespace RWBYRemnant
{
    public class ThingDef_RubyDash : ThingDef
    {
        public Color color;
    }

    public class Projectile_RubyDash : Bullet
    {
        public ThingDef_RubyDash Def
        {
            get
            {
                return def as ThingDef_RubyDash;
            }
        }

        public override void Tick()
        {
            if (launcher is Pawn launcherPawn && launcherPawn != null)
            {
                if (launcherPawn.Downed || launcherPawn.Dead)
                {
                    launcherPawn.health.hediffSet.hediffs.RemoveAll(h => h.def == RWBYDefOf.RWBY_RubyDashForm);
                    this.Destroy();
                    return;
                }
                Find.Selector.Deselect(launcher);
                Hediff dashHediff = new Hediff();
                dashHediff = HediffMaker.MakeHediff(RWBYDefOf.RWBY_RubyDashForm, launcherPawn);
                launcherPawn.health.AddHediff(dashHediff);
                launcherPawn.Position = Position;
                launcherPawn.Notify_Teleported(true, false);
            }
            MoteMaker.ThrowDustPuffThick(Position.ToVector3(), Map, 2, Def.color);
            if (Find.TickManager.TicksGame % 2 == 0)
            {
                if (!Position.ShouldSpawnMotesAt(Map))
                {
                    return;
                }
                MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(RWBYDefOf.RWBY_Rose_Petal, null);
                moteThrown.Scale = 1.9f;
                moteThrown.rotationRate = 0f;
                moteThrown.exactPosition = Position.ToVector3();
                moteThrown.SetVelocity((float)Rand.Range(0, 360), 0.2f);
                GenSpawn.Spawn(moteThrown, Position, Map, WipeMode.Vanish);
            }
            base.Tick();
        }

        protected override void Impact(Thing hitThing)
        {
            if (launcher is Pawn launcherPawn && launcherPawn != null)
            {
                launcherPawn.Position = Position;
                launcherPawn.Notify_Teleported(true, false);
                launcherPawn.health.hediffSet.hediffs.RemoveAll(h => h.def == RWBYDefOf.RWBY_RubyDashForm);
            }
            base.Impact(hitThing);
        }       
    }
}
