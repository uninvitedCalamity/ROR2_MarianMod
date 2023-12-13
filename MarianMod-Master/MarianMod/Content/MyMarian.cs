using BepInEx.Configuration;
using MarianMod.Modules.Characters;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MarianMod.Modules.Survivors
{
    internal class MyMarian : SurvivorBase
    {
        public override string bodyName => "Marian";

        public const string Marian_PREFIX = MarianPlugin.DEVELOPER_PREFIX + "_Marian_BODY_";
        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => Marian_PREFIX;

        public const string prefix = MarianPlugin.DEVELOPER_PREFIX;
        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "MarianBody",
            bodyNameToken = MarianPlugin.DEVELOPER_PREFIX + "_Marian_BODY_NAME",
            subtitleNameToken = MarianPlugin.DEVELOPER_PREFIX + "_Marian_BODY_SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("Marian_Face_Icon"),
            bodyColor = Color.white,

            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 110f,
            healthRegen = 1.5f,
            armor = 0f,
            moveSpeed = 6f,

            jumpCount = 1,
            damage = 6
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] 
        {
                new CustomRendererInfo
                {
                    childName = "Body",
                    material = Materials.CreateHopooMaterial("MarianUpperody"),
                },
                new CustomRendererInfo
                {
                    childName = "Legs",
                    material = Materials.CreateHopooMaterial("MarianLegs"),
                },
                new CustomRendererInfo
                {
                    childName = "Mouth",
                    material = Materials.CreateHopooMaterial("Eyes"),
                },
                new CustomRendererInfo
                {
                    childName = "Robotics",
                    material = Materials.CreateHopooMaterial("Robotics"),
                },
                new CustomRendererInfo
                {
                    childName = "Shorts",
                    material = Materials.CreateHopooMaterial("Shorts"),
                },
                new CustomRendererInfo
                {
                    childName = "MechaBra",
                    material = Materials.CreateHopooMaterial("Bra"),
                },
                new CustomRendererInfo
                {
                    childName = "Grapples",
                    material = Materials.CreateHopooMaterial("Bra"),
                }
        };

        public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(EntityStates.GenericCharacterMain);

        public override ItemDisplaysBase itemDisplays => new MarianItemDisplays();

                                                                          //if you have more than one character, easily create a config to enable/disable them like this
        public override ConfigEntry<bool> characterEnabledConfig => null; //Modules.Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
        }

        public override void InitializeUnlockables()
        {
            //uncomment this when you have a mastery skin. when you do, make sure you have an icon too
            //masterySkinUnlockableDef = Modules.Unlockables.AddUnlockable<Modules.Achievements.MasteryAchievement>();
        }

        public override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;

            //example of how to create a hitbox
            //Transform hitboxTransform = childLocator.FindChild("SwordHitbox");
            //Modules.Prefabs.SetupHitbox(model, hitboxTransform, "Sword");
        }

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);
            string prefix = Marian_PREFIX;

            #region Primary
            //Creates a skilldef for a typical primary 
            SkillDef primarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo(prefix + "_Marian_BODY_PRIMARY_SLASH_NAME",
                                                                                      prefix + "_Marian_BODY_PRIMARY_SLASH_DESCRIPTION",
                                                                                      Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                                                                                      new EntityStates.SerializableEntityStateType(typeof(SkillStates.SlashCombo)),
                                                                                      "Weapon",
                                                                                      true));


            //Modules.Skills.AddPrimarySkills(bodyPrefab, primarySkillDef);
            #endregion

            #region Secondary
            SkillDef shootSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SCATTERGUN",
                skillNameToken = prefix + "SCATTERGUN",
                skillDescriptionToken = prefix + "SCATTERGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ShotgunIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.PrimaryFire)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 0,
                baseRechargeInterval = 0.25f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 0,
            });

            SkillDef IceBomb = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "ICEBOMB",
                skillNameToken = prefix + "ICEBOMB",
                skillDescriptionToken = prefix + "ICEBOMB_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("IceGrenadeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.MarianIceBomb)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 17f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_AGILE" }
            });
            #endregion

            #region Utility
            SkillDef GrappleSwitch = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "GRAPPLESWITCH",
                skillNameToken = prefix + "GRAPPLESWITCH",
                skillDescriptionToken = prefix + "GRAPPLESWITCH_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("GrappleInitIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.GrappleSwitch)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_AGILE" }
            });
            SkillDef Lunge = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "GRAPPLESWITCH",
                skillNameToken = prefix + "GRAPPLESWITCH",
                skillDescriptionToken = prefix + "GRAPPLESWITCH_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("GrappleInitIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Marian_Lunge)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_AGILE" }
            });

            #endregion

            #region Special
            SkillDef MarianFlare = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "MISSILES",
                skillNameToken = prefix + "MISSILES",
                skillDescriptionToken = prefix + "MISSILE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("MissileIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.MarianFlare)),//Marian_ShatteringPierce)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 9f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = true,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Modules.Skills.AddPrimarySkills(bodyPrefab, shootSkillDef);
            Modules.Skills.AddSecondarySkills(bodyPrefab, MarianFlare);
            Modules.Skills.AddUtilitySkills(bodyPrefab, GrappleSwitch);
            //Modules.Skills.AddUtilitySkills(bodyPrefab, Lunge);
            Modules.Skills.AddSpecialSkills(bodyPrefab, IceBomb);
            #endregion
        }

        public override void InitializeSkins()
        {
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = characterModel.baseRendererInfos;
            Log.Debug("Render infos count = " + defaultRendererinfos.Length);


            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(Marian_PREFIX + "DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("Marian_Default"),
                defaultRendererinfos,
                model);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
            //pass in meshes as they are named in your assetbundle
            //defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRenderers,
            //    "meshMarianSword",
            //    "meshMarianGun",
            //    "meshMarian");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            
            SkinDef NoPantV2 = Modules.Skins.CreateSkinDef(Marian_PREFIX + "NOPANT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("Marian_noShorts"),
                defaultRendererinfos,
                model);

            Log.Debug("MeshRaplacements");
            NoPantV2.meshReplacements = Modules.Skins.getMeshReplacements(defaultRendererinfos,
                null,
                null,//no gun mesh replacement. use same gun mesh
                null,
                null,
                null,
                null,
                null);

            Log.Debug("RenderInfos");
            NoPantV2.rendererInfos[1].defaultMaterial = defaultSkin.rendererInfos[1].defaultMaterial;
            NoPantV2.rendererInfos[2].defaultMaterial = defaultSkin.rendererInfos[2].defaultMaterial;
            NoPantV2.rendererInfos[3].defaultMaterial = defaultSkin.rendererInfos[3].defaultMaterial;
            NoPantV2.rendererInfos[4].defaultMaterial = defaultSkin.rendererInfos[4].defaultMaterial;
            NoPantV2.rendererInfos[5].defaultMaterial = defaultSkin.rendererInfos[5].defaultMaterial;
            NoPantV2.rendererInfos[6].defaultMaterial = defaultSkin.rendererInfos[6].defaultMaterial;
            NoPantV2.rendererInfos[0].defaultMaterial = defaultSkin.rendererInfos[0].defaultMaterial;

            Log.Debug("Deactivating Shorts");
            NoPantV2.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = childLocator.FindChild("Shorts").gameObject,
                    shouldActivate = false,
                }
            };

            skins.Add(NoPantV2);

            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin
            /*
            //creating a new skindef as we did before
            SkinDef masterySkin = Modules.Skins.CreateSkinDef(MarianPlugin.DEVELOPER_PREFIX + "_Marian_BODY_MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                model,
                masterySkinUnlockableDef);

            //adding the mesh replacements as above. 
            //if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRendererinfos,
                "meshMarianSwordAlt",
                null,//no gun mesh replacement. use same gun mesh
                "meshMarianAlt");

            //masterySkin has a new set of RendererInfos (based on default rendererinfos)
            //you can simply access the RendererInfos defaultMaterials and set them to the new materials for your skin.
            masterySkin.rendererInfos[0].defaultMaterial = Modules.Materials.CreateHopooMaterial("matMarianAlt");
            masterySkin.rendererInfos[1].defaultMaterial = Modules.Materials.CreateHopooMaterial("matMarianAlt");
            masterySkin.rendererInfos[2].defaultMaterial = Modules.Materials.CreateHopooMaterial("matMarianAlt");

            //here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("GunModel"),
                    shouldActivate = false,
                }
            };
            //simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            skins.Add(masterySkin);
            */
            #endregion

            skinController.skins = skins.ToArray();
        }
    }
}