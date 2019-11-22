using RimWorld;
using UnityEngine;
using Verse;

namespace RWBYRemnant
{
    public class ThingDef_RubyCarry : ThingDef
    {
        public Color color;
    }

    public class Projectile_RubyCarry : Bullet
    {
        public ThingDef_RubyCarry Def
        {
            get
            {
                return def as ThingDef_RubyCarry;
            }
        }

        public Pawn carriedPawn;

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

                if (carriedPawn == null)
                {
                    foreach (Pawn pawn in Map.mapPawns.AllPawns)
                    {
                        if (pawn != launcherPawn && pawn.AdjacentTo8WayOrInside(launcherPawn)) carriedPawn = pawn;
                    }
                }
                if (carriedPawn != null)
                {
                    carriedPawn.Position = Position;
                    carriedPawn.Notify_Teleported(true, false);
                }
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
