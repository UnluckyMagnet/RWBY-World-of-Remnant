using RimWorld;
using UnityEngine;
using Verse;

namespace RWBYRemnant
{
    public class CompProperties_WeaponProjectile : CompProperties
    {
        public string AbilityLabel;
        public string AbilityDesc;
        public string AbilityIcon;
        public Color AbilityIconColor = new Color(1, 1, 1);
        public SoundDef AbilitySound;
        public ThingDef AbilityProjectile;
        public float WarmupTime;
        public float Range;
        public int BurstShotCount = 1;
        public int TicksBetweenBurstShots = 1;
        public bool cannotMiss = false;
        public TargetingParameters targetingParameters = null;
        public ThingDef usesAmmunition = null;

        public CompProperties_WeaponProjectile()
        {
            compClass = typeof(CompWeaponProjectile);
        }
    }
}