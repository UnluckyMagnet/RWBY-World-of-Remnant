using Verse;

namespace RWBYRemnant
{
    public class CompProperties_WeaponDrinkCoffee : CompProperties
    {
        public string AbilityLabel;
        public string AbilityDesc;
        public string AbilityIcon;
        public SoundDef AbilitySound;

        public CompProperties_WeaponDrinkCoffee()
        {
            compClass = typeof(CompWeaponDrinkCoffee);
        }
    }
}
