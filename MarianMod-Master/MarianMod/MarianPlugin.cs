using BepInEx;
using BepInEx.Configuration;
using MarianMod.Modules.Survivors;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//rename this namespace
namespace MarianMod
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin("com.uninvitedcalamity.MarianMod", "UninvitedCalamity_MarianMod", "1.2.2")]
    [BepInDependency("com.johnedwa.RTAutoSprintEx", BepInDependency.DependencyFlags.SoftDependency)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
        "UnlockableAPI"
    })]

    public class MarianPlugin : BaseUnityPlugin
    {
        // if you don't change these you're giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.uninvitedcalamity.MarianMod";
        public const string MODNAME = "UninvitedCalamity_MarianMod";
        public const string MODVERSION = "1.2.2";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "UninvitedCalamity";

        public static MarianPlugin instance;
        private static ConfigFile CustomConfigFile { get; set; }
        public static ConfigEntry<int> MyConfigEntry { get; set; }
        public static ConfigEntry<int> MySecondConfigEntry { get; set; }

        private int DisplaysEnabled = 1;

        private void Awake()
        {
            CustomConfigFile = new ConfigFile(Paths.ConfigPath + "\\com-uninvitedCalamity-Marian.cfg", true);
            MyConfigEntry = CustomConfigFile.Bind<int>(
                "ColourBlindness Section",
                "ColourBlindMode, default 0: set to 1 for Red-Green Blindness, 2 for Blue-Yellow Blindness",
                0,
                "1 for Red-Green Blindness, 2 for Blue-Yellow Blindness"
                );
            MyConfigEntry = CustomConfigFile.Bind<int>(
                "DisplaySection",
                "DisplayItems, default 1: Set to 1 for True, 0 for False",
                1,
                "Set to 1 for True, 0 for False"
                );

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.johnedwa.RTAutoSprintEx"))
            {
                SendMessage("RT_SprintDisableMessage", "MarianMod.SkillStates.PrimaryFire");
                SendMessage("RT_SprintDisableMessage", "MarianMod.SkillStates.MarianFlare");
            }


            instance = this;

            Log.Init(Logger);
            Modules.Assets.Initialize(); // load assets and read config
            Modules.Config.ReadConfig();
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            // survivor initialization
            new MyMarian().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            Hook();
        }

        private void Hook()
        {
            // run hooks here, disabling one is as simple as commenting out the line
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            // a simple stat hook, adds armor after stats are recalculated
            if (self)
            {
                /*
                if (self.HasBuff(Modules.Buffs.armorBuff))
                {
                    self.armor += 300f;
                }
                */
            }
        }
    }
}