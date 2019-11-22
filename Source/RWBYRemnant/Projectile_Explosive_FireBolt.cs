using RimWorld;
using Verse;

namespace RWBYRemnant
{
    public class Myrtenaster_FireBolt : ThingDef
    {
        public ThingDef spawnWhileFlying;
        public int fireDistanceInTicks;
    }

    public class Projectile_Explosive_FireBolt : Projectile_Explosive
    {
        public Myrtenaster_FireBolt Def
        {
            get
            {
                return def as Myrtenaster_FireBolt;
            }
        }

        public int currentTick = 0;

        public override void Tick()
        {
            if (currentTick > Def.fireDistanceInTicks && Find.TickManager.TicksGame % 2 == 0)
            {
                FilthMaker.MakeFilth(Position, Map, Def.spawnWhileFlying);
                Fire fire = (Fire)ThingMaker.MakeThing(ThingDefOf.Fire);
                fire.fireSize = 1f;
                GenSpawn.Spawn(fire, Position, Map);
            }
            else
            {
                currentTick++;
            }
            base.Tick();
        }
    }
}
