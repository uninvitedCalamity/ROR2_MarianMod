using System;
using System.Collections.Generic;
using System.Text;
using MarianMod;
using RoR2;

namespace MarianMod.Modules.Unlocks
{
    class MarianMasteryUnlock : BaseMasteryUnlockable
    {
        public override string AchievementTokenPrefix => MarianPlugin.DEVELOPER_PREFIX + "_MARIAN_BODY_MASTERY";
        public override string AchievementSpriteName => "texMasterySkin";
        public override string PrerequisiteUnlockableIdentifier => MarianPlugin.DEVELOPER_PREFIX + "_MARIAN_BODY_UNLOCKABLE_REWARD_ID";
        public override string RequiredCharacterBody => "MarianBody";
        public override float RequiredDifficultyCoefficient => 3;
    }
}
