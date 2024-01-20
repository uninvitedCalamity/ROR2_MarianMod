using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using System.Collections.Generic;

namespace MarianMod.SkillStates
{
    class Marian_Grapple_simple: BaseSkillState
    {
        Vector3 targetOffset;
        Transform targetPoint;
        Ray BaseRay;
        ChildLocator locator;
        Transform modelTransform;
        float initialBoost = 0.1f;
        const float defaultMax = 7;
        float maxVelocity = defaultMax;
        float interpolateBoost = 1;
        //float distance;
        float minDistance = 1f;
        float delay = 0.1f;
        bool lunge = false;
        Vector3 GrapplePos;
        Transform grapple;
        Vector3 preVel = new Vector3(0,0,0);
        public static float damageCoef = 20f;

        float duration = 0.25f;
        Vector3 hitPoint;
        float dampenZone = 10f;
        static public float distance = 100;
        bool hasAnimated = false;
        bool jump = false;
        bool hasHealthComponent;
        bool enteredCollision = false;
        Vector3 pos = Vector3.zero;
        bool failed = false;
        bool animationEnded = false;
        int anim = 0;

        public HealthComponent upwardSearch(GameObject observing, bool searching)
        {
            bool ChildFound = false;
            HealthComponent returnable = null;
            do
            {
                HurtBox h1;
                HealthComponent h;
                if (h = observing.GetComponent<HealthComponent>())
                {
                    ChildFound = true;
                    returnable = h;
                    searching = false;
                }
                else if (h1 = observing.GetComponent<HurtBox>())
                {
                    if (h1.healthComponent != null)
                    {
                        ChildFound = true;
                        returnable = h1.healthComponent;
                        searching = false;
                    }
                }
                else
                {
                    if (observing.transform.parent != null)
                        observing = observing.transform.parent.gameObject;
                    else
                        searching = false;
                }
            } while (searching);
            return returnable;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            //distance = Mathf.Infinity;
            BaseRay = base.GetAimRay();
            RaycastHit raycastHit;
            float num = distance;
            this.modelTransform = base.GetModelTransform();
            this.locator = modelTransform.GetComponent<ChildLocator>();
            BaseRay.origin += (BaseRay.direction * 0.5f);
            Vector3 vector = Vector3.zero;
            grapple = locator.FindChild("LeftGrapple").transform;
            GrapplePos = grapple.localPosition;
            if (Physics.Raycast(this.BaseRay, out raycastHit, num, LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask))
            {
                base.characterMotor.Motor.ForceUnground();
                vector = raycastHit.point;
                Log.Debug(raycastHit.collider.name);
                targetPoint = raycastHit.collider.transform;
                targetOffset = targetPoint.InverseTransformPoint(raycastHit.point);
                hitPoint = raycastHit.point;                
                grapple.position = hitPoint;
                HealthComponent returned;
                if (returned = upwardSearch(raycastHit.collider.gameObject, true))
                {
                    hasHealthComponent = true;
                    if (returned.body == base.characterBody)
                    {
                        Log.Debug("Hit Marian");
                        RaycastHit hit2;
                        Vector3 recast = raycastHit.point + (this.BaseRay.direction * 0.5f);
                        Ray ray2 = new Ray();
                        ray2.origin = recast;
                        ray2.direction = this.BaseRay.direction;

                        if (Physics.Raycast(ray2, out hit2, num, LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask))
                        {
                            vector = hit2.point;
                            Log.Debug(hit2.collider.name);
                            targetPoint = hit2.collider.transform;
                            targetOffset = targetPoint.InverseTransformPoint(hit2.point);
                            hitPoint = hit2.point;
                            //EffectManager.SimpleEffect(EntityStates.GolemMonster.FireLaser.hitEffectPrefab, hit2.point, new Quaternion(0, 0, 0, 0), true);
                            grapple.position = hitPoint;
                            if (returned = upwardSearch(hit2.collider.gameObject, true))
                            {
                                hasHealthComponent = true;
                            }
                        }
                    }

                }
                else
                    hasHealthComponent = false;
                Log.Debug("Start GrappleAnim");
                EffectManager.SimpleEffect(EntityStates.GolemMonster.FireLaser.hitEffectPrefab, hitPoint, new Quaternion(0, 0, 0, 0), true);
            }
            else
            {
                Log.Debug("Out of range");
                failed = true;
                anim = 2;
                outer.SetNextStateToMain();
            }
            if (targetPoint == null)
            {
                failed = true;
                anim = 2;
                outer.SetNextStateToMain();
            }
            //base.PlayAnimation("Grapple", "Grapple", "Grapple.playbackRate", 1);

        }

        public void exitAnim(int anim)
        {
            if(anim == 1)
                base.PlayAnimation("Grapple", "GrappleToKick");
            else if (anim == 0)
                base.PlayAnimation("Grapple", "TestState");            
        }
        public override void OnExit()
        {
            if (jump)
            {
                if(base.isAuthority)
                    base.characterMotor.velocity *= 1.15f;
            }
            else if (hasHealthComponent && enteredCollision)
            {
                anim = 1;
                base.GetComponent<MarianMod.Modules.CharacterDataStore>().kicked = true;
                if (base.isAuthority)
                {
                    Vector3 Overrride = -base.characterMotor.velocity * 0.5f;
                    Overrride.y = 14;
                    base.characterMotor.velocity = Overrride;
                    new BlastAttack
                    {
                        attacker = base.gameObject,
                        inflictor = base.gameObject,
                        teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
                        baseDamage = damageCoef * base.damageStat,
                        baseForce = 0.5f * 0.2f,
                        position = grapple.position,
                        radius = 3,
                        falloffModel = BlastAttack.FalloffModel.SweetSpot,
                        damageType = DamageType.Stun1s,
                    }.Fire();
                }
                EffectManager.SimpleEffect(EntityStates.GolemMonster.FireLaser.hitEffectPrefab, grapple.position, new Quaternion(0, 0, 0, 0), true);
            }
            grapple.localPosition = GrapplePos;
            exitAnim(anim);
            base.OnExit();
        }

        public void swingGrapple()
        {
            if (!modelTransform)
            {
                outer.SetNextStateToMain();
                return;
            }

            if (base.fixedAge < delay)
                return;

            if (base.inputBank.jump.justPressed)
            {
                base.characterMotor.velocity += Vector3.up * 14;
                jump = true;
            }
            if (!base.inputBank.skill1.down || jump)
            {
                outer.SetNextStateToMain();
                return;
            }
            if (enteredCollision)
                outer.SetNextStateToMain();
            if (hasHealthComponent)
                if (targetPoint.transform.gameObject == null)
                    outer.SetNextStateToMain();

            if (targetPoint != null)
            {
                if (!hasAnimated)
                {
                    base.PlayAnimation("Grapple", "Jump3To GrappleStart");
                    hasAnimated = true;
                }
                base.characterMotor.Motor.ForceUnground();

                Vector3 direction = base.transform.position - pos;

                float boost = maxVelocity;
                Vector3 Velocity = direction.normalized * boost;//base.characterMotor.velocity - (direction.normalized * boost);
                Vector3 difference = (base.transform.position + Velocity) - pos;

                float boost_mod = boost;
                float CurrentDistance = Vector3.Distance(pos, base.transform.position);
                if (CurrentDistance < dampenZone)
                    if (!hasHealthComponent)
                        boost_mod = CurrentDistance / dampenZone;
                    else if (CurrentDistance < dampenZone * 0.6f)
                        enteredCollision = true;
                Velocity *= -boost_mod;

                Vector3 outputVel = Velocity;
                if (outputVel.magnitude > maxVelocity)
                    outputVel = outputVel.normalized * maxVelocity;
                base.characterMotor.velocity = outputVel;
            }
            else
                outer.SetNextStateToMain();
            preVel = base.characterMotor.velocity;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (targetPoint != null)
                pos = targetPoint.TransformPoint(targetOffset);
            else
            {
                outer.SetNextStateToMain();
                return;
            }
            grapple.position = pos;
            if (targetPoint != null && !base.isAuthority)
            {
                if (!hasAnimated)
                {
                    base.PlayAnimation("Grapple", "Jump3To GrappleStart");
                    hasAnimated = true;
                }
            }
            if (targetPoint == null)
            {
                base.GetComponent<MarianMod.Modules.CharacterDataStore>().exitSwitch = true;
                outer.SetNextStateToMain();
                return;
            }
            if (base.isAuthority)
            {
                base.characterBody.SetAimTimer(2f);
                maxVelocity = defaultMax * (base.moveSpeedStat);
                swingGrapple();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
