using Verse;

namespace RWBYRemnant
{
    [StaticConstructorOnStartup]
    static class ModChecker
    {
        static ModChecker()
        {
            CheckForRWBYWeapons();
        }        

        public static void CheckForRWBYWeapons()
        {
            bool rwbyWeaponsLoaded = false;
            foreach (ModContentPack modContentPack in LoadedModManager.RunningMods)
            {
                if (modContentPack.Name == "RWBY Weapons")
                {
                    rwbyWeaponsLoaded = true;
                }
            }
            if (rwbyWeaponsLoaded)
            {
                Log.Error("RWBY Weapons is incompatible with RWBY World of Remnant, because it is already integrated in RWBY World of Remnant");
            }
        }
    }
}
