﻿using R2API;
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
            desc = desc + "< ! > Smart missile barrage will try to evenly distribute missiles between enemies on screen. Smart missile barrage will increase targeting count and missile count while held and targeting enemies" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Grappling hook Changes your primary skill to a grappling hook, releasing skill 1 or pressing skill 3 ends Grappling hook" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Jumping while ingrappling hook gives you a increases your velocity and boosts you upward"; 

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
            LanguageAPI.Add(prefix + "SCATTERGUN_DESCRIPTION", $"Fire 5 bullets for <style=cIsDamage>5*{100f * MarianMod.SkillStates.PrimaryFire.DamageCoef}% damage</style> each, full auto.");
            #endregion

            #region Primary
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "ICEBOMB", "Ice bomb");
            LanguageAPI.Add(prefix + "ICEBOMB_DESCRIPTION", $"Throw a grenade, dealing <style=cIsDamage>{100f * MarianMod.SkillStates.MarianIceBomb.DamageCoef}% damage</style> and freezing enemies over {2 * MarianMod.Modules.Projectiles.iceProc} seconds.");

            LanguageAPI.Add(prefix + "POISONBOMB", "Poison bomb");
            LanguageAPI.Add(prefix + "POISONBOMB_DESCRIPTION", $"Throw a grenade, dealing <style=cIsDamage>{100f * MarianMod.SkillStates.MarianIceBomb.DamageCoef * 50}% damage</style> and poisoning enemies for 10 seconds.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "GRAPPLESWITCH", "Grappling hook");
            LanguageAPI.Add(prefix + "GRAPPLESWITCH_DESCRIPTION", $"Fire a grappling hook from primary, pulling directly to the grappling hook. If grapplng to an enemy, stun and deal <style=cIsDamage>{100f * MarianMod.SkillStates.Marian_Grapple_simple.damageCoef}% damage</style>.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_BOMB_NAME", "Bomb");
            LanguageAPI.Add(prefix + "SPECIAL_BOMB_DESCRIPTION", $"Throw a bomb for <style=cIsDamage>{100f * StaticValues.bombDamageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "MISSILES", "Poison smart missile barrage");
            LanguageAPI.Add(prefix + "MISSILE_DESCRIPTION", $"Fire 5 missiles for <style=cIsDamage>5*{100f * MarianMod.SkillStates.MarianFlare.DamageCoef}% damage</style> afflicting poison.");


            LanguageAPI.Add(prefix + "MISSILESICE", "Ice smart missile barrage");
            LanguageAPI.Add(prefix + "MISSILEICE_DESCRIPTION", $"Fire 5 missiles for <style=cIsDamage>5*{100f * MarianMod.SkillStates.MarianFlare.DamageCoef}% damage</style> afflicting freeze.");
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