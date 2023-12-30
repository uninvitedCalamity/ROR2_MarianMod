using MarianMod.Modules;

namespace MarianMod.Achievements
{
    public class MarianMasteryAchievement : BaseMasteryUnlockable
    {
        public override string AchievementTokenPrefix => MarianPlugin.DEVELOPER_PREFIX + "_Marian_BODY_MASTERY";
        public override string PrerequisiteUnlockableIdentifier => "MARIAN_CHARACTERUNLOCKABLE_ACHIEVEMENT_ID";
        public override string AchievementSpriteName => "texMasterySkin";

        public override string RequiredCharacterBody => "MarianBody";

        public override float RequiredDifficultyCoefficient => 3f;
    }
}
