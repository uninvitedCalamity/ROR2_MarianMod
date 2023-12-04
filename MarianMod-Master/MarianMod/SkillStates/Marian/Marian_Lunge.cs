using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using System.Collections.Generic;


namespace MarianMod.SkillStates
{
    class Marian_Lunge : BaseSkillState
    {
        Ray BaseRay;
        Vector3 targetPoint;
        float delay = 0.25f;
        float duration = 0.5f;
        float movementSpeed = 1;
        float maxvelocity = 70;
        ChildLocator locator;
        Transform modelTransform;
        float slowdownRange = 2;

        Vector3 GrapplePos;
        Transform grapple;

        public override void OnEnter()
        {
            base.OnEnter();
            BaseRay = base.GetAimRay();
            RaycastHit raycastHit;
            float num = 100;
            base.characterMotor.Motor.ForceUnground();
            this.modelTransform = base.GetModelTransform();
            this.locator = modelTransform.GetComponent<ChildLocator>();
            BaseRay.origin += (BaseRay.direction*0.5f);
            grapple = locator.FindChild("LeftGrapple").transform;
            GrapplePos = grapple.localPosition;
            if (Physics.Raycast(this.BaseRay, out raycastHit, num, LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask))
            {
                targetPoint = raycastHit.point;    
                EffectManager.SimpleEffect(EntityStates.GolemMonster.FireLaser.hitEffectPrefab, raycastHit.point, new Quaternion(0, 0, 0, 0), true);
                grapple.position = targetPoint;
            }
            else
            {
                outer.SetNextStateToMain();
            }
            movementSpeed *= base.moveSpeedStat;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                //For target pos, access the script holding where this wants to go
                Vector3 Difference1 = base.transform.position - targetPoint;
                Difference1 = Difference1.normalized * moveSpeedStat;
                Vector3 Difference2 = Difference1 * movementSpeed;
                float timeScale = base.fixedAge / duration;
                Vector3 Interpolated = Difference2 * (timeScale) + (1 - (timeScale)) * Vector3.zero;
                if (base.fixedAge >= delay)
                {
                    grapple.transform.position = targetPoint;
                    base.characterMotor.Motor.ForceUnground();
                    Vector3 newVec = base.characterMotor.velocity - Interpolated;
                    float velMeasure = maxvelocity;
                    float Progress = Vector3.Distance(base.transform.position, targetPoint);
                    if (Progress <= slowdownRange)
                    {
                        velMeasure = Progress / slowdownRange * maxvelocity;
                    }
                    if (newVec.magnitude > velMeasure)
                        newVec = newVec.normalized * velMeasure;
                    base.characterMotor.velocity = newVec;//-Interpolated;
                }
                if (base.fixedAge >= duration)
                    outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            grapple.localPosition = GrapplePos;
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
