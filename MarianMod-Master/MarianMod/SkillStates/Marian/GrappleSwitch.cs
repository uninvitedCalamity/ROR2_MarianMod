using EntityStates;
using RoR2;
using RoR2.Skills;
using RoR2.UI;
using UnityEngine;
using System.Collections.Generic;

namespace MarianMod.SkillStates
{
    class GrappleSwitch : BaseSkillState
    {

        public GenericSkill overriddenSkill;
        public SkillDef primaryOverride;
        public float duration = 0.3f;
        public float windup;
        bool m1Pressed = false;
        bool skill3Released = false;
        public static GameObject crosshairOverridePrefab;
        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        private CrosshairUtils.OverrideRequest crosshairOverrideRequestSprint;
        Ray BaseRay;
        public Transform Target;
        Vector3 HitPoint = new Vector3(0,0,0);
        static Vector3 Largest = new Vector3(0.005f, 0.005f, 0.005f);
        Vector3 Smallest = Largest / 3.25f;
        GameObject Camera;
        float Size = 0;
        const float scale = 0.25f;
        Vector3 BaseScale = new Vector3(scale, scale, scale);
        bool shrink = false;


        public override void OnEnter()
        {
            base.OnEnter();
            //Change skill 1 to Grapple
            primaryOverride = Modules.Skills.CreateSkillDef(new Modules.SkillDefInfo
            {
                skillName = MarianMod.Modules.Survivors.MyMarian.prefix + "_Marian_BODY_GRAPPLE",
                skillNameToken = MarianMod.Modules.Survivors.MyMarian.prefix + "_Marian_BODY_GRAPPLE",
                skillDescriptionToken = MarianMod.Modules.Survivors.MyMarian.prefix + "_Marian_BODY_GRAPPLE",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("GrappleIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Marian_Grapple_simple)),
                activationStateMachineName = "Body",
                baseMaxStock = 0,
                baseRechargeInterval = 0.25f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 0,
                keywordTokens = new string[] { "KEYWORD_AGILE" }
            });

            SkillLocator skilllocator = base.skillLocator;
            GenericSkill genericSkill = skilllocator.primary;
            if (genericSkill)
            {
                this.TryOverrideSkill(genericSkill);
                genericSkill.onSkillChanged += this.TryOverrideSkill;
            }
            Log.Debug("OverriddenSkill");
            Camera = GameObject.Find("Main Camera(Clone)");
            if (Target == null && !m1Pressed)
                Target = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.GrappleSprite, Camera.transform.position, Quaternion.identity).transform;
            Target.transform.parent = Camera.transform;
            Target.transform.rotation = Camera.transform.rotation;
            Target.transform.localScale = BaseScale * Size;
            Target.localPosition = new Vector3(0, 0, 0.1f); // <- make this change based on how close the hit point is to the max distance.
        }

        public override void OnExit()
        {
            releaseGui();
            CrosshairUtils.OverrideRequest overrideRequest = this.crosshairOverrideRequest;
            if (overrideRequest != null)
            {
                overrideRequest.Dispose();
            }
            overrideRequest = this.crosshairOverrideRequestSprint;
            if (overrideRequest != null)
            {
                overrideRequest.Dispose();
            }
            Log.Debug("Skill Unassigned");
            base.OnExit();
        }

        bool EXIT = false;
        bool TimerAssigned = false;
        float timer = 0.3f;
        float timeAtExit;

        public void releaseGui()
        {
            
            if (Target != null)
            {
                Destroy(Target.gameObject);
                Target = null;
            }
            
        }
        float yOffset = 0.0275f;
        float xOffset = 0.05f;
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!base.isAuthority)
                return;

            Target.transform.localScale = BaseScale * Size;
            if(!shrink)
                Size = Mathf.Clamp(Size + (Time.deltaTime * (1/0.3f)),0,1);

            RaycastHit raycastHit;
            BaseRay = base.GetAimRay();
            BaseRay.origin += (BaseRay.direction * 0.5f);
            if (Physics.Raycast(this.BaseRay, out raycastHit, Mathf.Infinity, LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask))
            {
                CrosshairUtils.OverrideRequest overrideRequest = this.crosshairOverrideRequest;
                if(overrideRequest == null && !m1Pressed)
                    if (EntityStates.Engi.EngiMissilePainter.Paint.crosshairOverridePrefab)
                    {
                        this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, EntityStates.Huntress.AimArrowSnipe.crosshairOverridePrefab, CrosshairUtils.OverridePriority.Skill);
                        //this.crosshairOverrideRequestSprint = CrosshairUtils.RequestOverrideForBody(base.characterBody, EntityStates.Engi.EngiMissilePainter.Paint.crosshairOverridePrefab, CrosshairUtils.OverridePriority.Sprint);
                    }

                HitPoint = raycastHit.point;

                //:) Put the GUI Cheesingm at the start and end of this ability
                float max = Marian_Grapple_simple.distance;
                if (Target != null)
                {                    
                    float distance = Vector3.Distance(BaseRay.origin, raycastHit.point);
                    if (distance <= Marian_Grapple_simple.distance)
                    {
                        float percent1 = distance / Marian_Grapple_simple.distance;
                        float percent2 = 1 - percent1;
                        //Target.GetChild(0).transform.localScale = (Largest * percent2) + (Smallest * percent1);
                        Target.GetChild(1).transform.localPosition = (Vector3.zero * percent2) + ((Vector3.up * yOffset) * percent1);
                        Target.GetChild(2).transform.localPosition = (Vector3.zero * percent2) + ((Vector3.down * yOffset) * percent1);
                        Target.GetChild(3).transform.localPosition = (Vector3.zero * percent2) + ((Vector3.left * -xOffset) * percent1);
                        Target.GetChild(4).transform.localPosition = (Vector3.zero * percent2) + ((Vector3.right * -xOffset) * percent1);

                        Target.GetChild(1).transform.localScale = Vector3.zero;
                        Target.GetChild(2).transform.localScale = Vector3.zero;

                        recolourRenderer(Target.transform.GetChild(0).gameObject, percent1);
                        recolourRenderer(Target.transform.GetChild(1).gameObject, percent1);
                        recolourRenderer(Target.transform.GetChild(2).gameObject, percent1);
                        recolourRenderer(Target.transform.GetChild(3).gameObject, percent1);
                        recolourRenderer(Target.transform.GetChild(4).gameObject, percent1);
                        //SpriteRenderer renderer = uiCom.GetComponent<SpriteRenderer>();
                        //Color lerpedColor = Color.Lerp(Color.HSVToRGB(0.3f, 1, 1), Color.HSVToRGB(0, 1, 1), percent1);
                        //renderer.material.color = lerpedColor;
                    }
                    else
                    {
                        //float distanceDifference = distance - Marian_Grapple_simple.distance;
                        float percent1 = distance / Marian_Grapple_simple.distance;
                        Target.GetChild(1).transform.localPosition = ((Vector3.up * yOffset) * percent1);
                        Target.GetChild(2).transform.localPosition = ((Vector3.down * yOffset) * percent1);
                        Target.GetChild(3).transform.localPosition = ((Vector3.left * -xOffset) * percent1);
                        Target.GetChild(4).transform.localPosition = ((Vector3.right * -xOffset) * percent1);

                        Target.GetChild(1).transform.localScale = Vector3.zero;
                        Target.GetChild(2).transform.localScale = Vector3.zero;


                        recolourRenderer(Target.transform.GetChild(0).gameObject, 1, true);
                        recolourRenderer(Target.transform.GetChild(1).gameObject, 1, true);
                        recolourRenderer(Target.transform.GetChild(2).gameObject, 1, true);
                        recolourRenderer(Target.transform.GetChild(3).gameObject, 1, true);
                        recolourRenderer(Target.transform.GetChild(4).gameObject, 1, true);
                    }
                }

                overrideRequest = this.crosshairOverrideRequestSprint;
            }
            else
            {
                if (Target != null)
                {
                    Target.GetChild(1).transform.localPosition = ((Vector3.up * yOffset) * 100);
                    Target.GetChild(2).transform.localPosition = ((Vector3.down * yOffset) * 100);
                    Target.GetChild(3).transform.localPosition = ((Vector3.left * -xOffset) * 100);
                    Target.GetChild(4).transform.localPosition = ((Vector3.right * -xOffset) * 100);

                    Target.GetChild(1).transform.localScale = Vector3.zero;
                    Target.GetChild(2).transform.localScale = Vector3.zero;

                    recolourRenderer(Target.transform.GetChild(0).gameObject, 1, true);
                    recolourRenderer(Target.transform.GetChild(1).gameObject, 1, true);
                    recolourRenderer(Target.transform.GetChild(2).gameObject, 1, true);
                    recolourRenderer(Target.transform.GetChild(3).gameObject, 1, true);
                    recolourRenderer(Target.transform.GetChild(4).gameObject, 1, true);
                }
                //releaseGui();
            }

            //if (m1Pressed)
                //releaseGui();       

            if (shrink)
            {
                Size = Mathf.Clamp(Size - (Time.fixedDeltaTime * (1 / 0.3f)), 0, 1);
            }

            if (EXIT && !TimerAssigned)
            {
                timeAtExit = base.fixedAge;
                TimerAssigned = true;
            }
            if (TimerAssigned)
            {
                if (base.fixedAge >= timeAtExit + timer && Size <= 0)
                    outer.SetNextStateToMain();
            }

            if (base.inputBank.skill1.justReleased)
                if (m1Pressed)
                {
                    EXIT = true;
                    shrink = true;
                }
            if (base.inputBank.skill1.justPressed)
            {
                m1Pressed = true;
                shrink = true;
            }
            if (base.fixedAge > duration)
                if (base.inputBank.skill3.justPressed)
                {
                    shrink = true;
                    EXIT = true;
                }

            if (EXIT)
            {
                shrink = true;
                UnAssign();
            }

        }

        public void recolourRenderer(GameObject uiCom, float percent1, bool outOfBounds = false)
        {
            float minvar = 0.5f;
            if (outOfBounds)
                minvar = 2;
            SpriteRenderer renderer = uiCom.GetComponent<SpriteRenderer>();
            Color lerpedColor = Color.Lerp(Color.HSVToRGB(0.3f, 1, 0.8f), Color.HSVToRGB(0f, 1, minvar), percent1);
            renderer.material.color = lerpedColor;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        private void TryOverrideSkill(GenericSkill skill)
        {
            if (skill && !this.overriddenSkill && !skill.HasSkillOverrideOfPriority(GenericSkill.SkillOverridePriority.Contextual))
            {
                this.overriddenSkill = skill;
                this.overriddenSkill.SetSkillOverride(this, primaryOverride, GenericSkill.SkillOverridePriority.Contextual);
                this.overriddenSkill.stock = base.skillLocator.primary.stock;
            }
            else
            {
                Log.Debug("Override is lacking");
            }
        }

        public void UnAssign()
        {
            SkillLocator skillLocator = base.skillLocator;
            GenericSkill genericSkill = (skillLocator != null) ? skillLocator.primary : null;
            if (genericSkill)
            {
                genericSkill.onSkillChanged -= this.TryOverrideSkill;
            }
            if (this.overriddenSkill)
            {
                this.overriddenSkill.UnsetSkillOverride(this, this.primaryOverride, GenericSkill.SkillOverridePriority.Contextual);
            }
        }
    }
}
