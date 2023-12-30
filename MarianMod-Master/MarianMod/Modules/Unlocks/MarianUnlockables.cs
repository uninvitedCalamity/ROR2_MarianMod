using RoR2;
using R2API;
using MarianMod.Achievements;
using MarianMod;


namespace MarianMod.Modules
{

    class MarianUnlockables
    {
        public static UnlockableDef MarianMasteryUnlockableDef;

        public static void RegisterUnlockables()
        {
            MarianMasteryUnlockableDef = UnlockableAPI.AddUnlockable<MarianMasteryAchievement>();
        }
    }
}
