using R2API;
using System;

namespace MarianMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region Marian
            string prefix = MarianPlugin.DEVELOPER_PREFIX + "_Marian_BODY_";

            string desc = "Marian shoots people.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Smart missile barrage will try to evenly distribute missiles between enemies on screen." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Grappling hook Changes your primary skill to a grappling hook, releasing skill 1 or pressing skill 3 ends Grappling hook";

            string outro = "..and so she left, reminded of that one time...";
            string outroFailure = "..if we call Minos Prime Pinos, doesn't that make Sisyphus Prime Pisyphus?";

            LanguageAPI.Add(prefix + "NAME", "Marian");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "Cyborg Snake");
            LanguageAPI.Add(prefix + "LORE", "She's big buff snake Lady and kills things.");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            LanguageAPI.Add(prefix + "NOPANT_SKIN_NAME", "No Pant");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Marian passive");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Sample text.");

            LanguageAPI.Add(prefix + "SCATTERGUN", "Scatter gun");
            LanguageAPI.Add(prefix + "SCATTERGUN_DESCRIPTION", $"Fire 5 bullets for <style=cIsDamage>{100f * MarianMod.SkillStates.PrimaryFire.DamageCoef}% damage</style> each, full auto.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_NAME", "Sword");
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_DESCRIPTION", Helpers.agilePrefix + $"Swing forward for <style=cIsDamage>{100f * StaticValues.swordDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_GUN_NAME", "Handgun");
            LanguageAPI.Add(prefix + "SECONDARY_GUN_DESCRIPTION", Helpers.agilePrefix + $"Fire a handgun for <style=cIsDamage>{100f * StaticValues.gunDamageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "ICEBOMB", "Ice bomb");
            LanguageAPI.Add(prefix + "ICEBOMB_DESCRIPTION", $"Fire a grenade, freezing enemies for <style=cIsDamage>{100f * MarianMod.SkillStates.MarianIceBomb.DamageCoef}% damage</style> over {2 * MarianMod.Modules.Projectiles.iceProc}.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_ROLL_NAME", "Roll");
            LanguageAPI.Add(prefix + "UTILITY_ROLL_DESCRIPTION", "Roll a short distance, gaining <style=cIsUtility>300 armor</style>. <style=cIsUtility>You cannot be hit during the roll.</style>.");

            LanguageAPI.Add(prefix + "GRAPPLESWITCH", "Grappling hook");
            LanguageAPI.Add(prefix + "GRAPPLESWITCH_DESCRIPTION", $"Fire a grappling hook from primary, pulling directly to the grappling hook. If grapplng to an enemy, stun and deal <style=cIsDamage>{100f * MarianMod.SkillStates.Marian_Grapple_simple.damageCoef}% damage</style>.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_BOMB_NAME", "Bomb");
            LanguageAPI.Add(prefix + "SPECIAL_BOMB_DESCRIPTION", $"Throw a bomb for <style=cIsDamage>{100f * StaticValues.bombDamageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "MISSILES", "Smart missile barrage");
            LanguageAPI.Add(prefix + "MISSILE_DESCRIPTION", $"Fire 5 missiles for <style=cIsDamage>{100f * MarianMod.SkillStates.MarianFlare.DamageCoef}% damage</style> afflicting Blight.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Marian: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Marian, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Marian: Mastery");
            #endregion
            #endregion
        }
    }
}