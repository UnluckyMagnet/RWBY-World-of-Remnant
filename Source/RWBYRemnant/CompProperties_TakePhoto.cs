using UnityEngine;
using Verse;

namespace RWBYRemnant
{
    public class CompProperties_TakePhoto : CompProperties
    {
        public Color LightCopyColor;
        public ThingDef usesAmmunition;

        public CompProperties_TakePhoto()
        {
            compClass = typeof(CompTakePhoto);
        }
    }
}
