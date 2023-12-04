using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using System.Collections.Generic;

namespace MarianMod.SkillStates
{
    class Marian_Grapple : BaseSkillState
    {
        Vector3 targetOffset;
        Transform targetPoint;
        Ray BaseRay;
        ChildLocator locator;
        Transform modelTransform;
        float initialBoost = 0.1f;
        float interpolateBoost = 1;
        float maxVelocity = 25;
        float distance;
        float minDistance = 1f;
        float delay = 0.25f;
        bool lunge = false;
        Vector3 GrapplePos;
        Transform grapple;
        Vector3 preVel = new Vector3(0,0,0);

        float duration = 0.25f;
        Vector3 hitPoint;

        public override void OnEnter()
        {
            base.OnEnter();
            distance = Mathf.Infinity;
            BaseRay = base.GetAimRay();
            RaycastHit raycastHit;
            float num = 500;
            base.characterMotor.Motor.ForceUnground();
            this.modelTransform = base.GetModelTransform();
            this.locator = modelTransform.GetComponent<ChildLocator>();
            BaseRay.origin += (BaseRay.direction * 0.5f);
            Vector3 vector = Vector3.zero;
            grapple = locator.FindChild("LeftGrapple").transform;
            GrapplePos = grapple.localPosition;
            if (Physics.Raycast(this.BaseRay, out raycastHit, num, LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask))
            {
                vector = raycastHit.point;
                Log.Debug(raycastHit.collider.name);
                targetPoint = raycastHit.collider.transform;
                targetOffset = targetPoint.InverseTransformPoint(raycastHit.point);
                hitPoint = raycastHit.point;
                EffectManager.SimpleEffect(EntityStates.GolemMonster.FireLaser.hitEffectPrefab, raycastHit.point, new Quaternion(0, 0, 0, 0), true);
                grapple.position = hitPoint;
            }
            else
            {
                outer.SetNextStateToMain();
            }
            Log.Debug("Grapple start");

        }

        public override void OnExit()
        {
            Log.Debug("Grapple Exit");
            base.OnExit();
        }

        public void swingGrapple()
        {
            Vector3 pos = targetPoint.TransformPoint(targetOffset);
            grapple.position = pos;

            SkillLocator skilllocator = base.skillLocator;
            GenericSkill genericSkill = skilllocator.primary;
            //if(skilllocator.primary)

            if (base.inputBank.skill1.justReleased)
            {
                //Vector3 pos = targetPoint.TransformPoint(targetOffset);
                Vector3 velocity = base.characterMotor.velocity;
                float distanceToPos = Vector3.Distance(pos, base.transform.position);
                float distanceToVelocity = Vector3.Distance(velocity.normalized * distanceToPos, base.transform.position);
                float distanceFromVelToPos = Vector3.Distance(pos, velocity.normalized * distanceToPos);
                float totalDistance = distanceToPos + distanceToVelocity + distanceFromVelToPos;

                if (totalDistance > 0)
                {
                    distanceToPos /= totalDistance;
                    distanceToVelocity /= totalDistance;
                    distanceFromVelToPos /= totalDistance;

                    // Used this site to check angles https://www.geogebra.org/m/JHgTXKrt
                    if (distanceFromVelToPos > 0.23f/*35 degrees */ && distanceFromVelToPos < 0.49f /*170 degrees */)
                    {
                        Vector3 v = base.characterMotor.velocity;
                        v.y *= 3;
                        base.characterMotor.velocity *= 2;
                    }
                }
                grapple.localPosition = GrapplePos;
                //outer.SetNextStateToMain();
            }
            if (base.fixedAge < delay)
                return;

            if (targetPoint != null)
            {
                base.characterMotor.Motor.ForceUnground();

                Vector3 direction = base.transform.position - pos;

                float boost = 1;
                if (base.fixedAge < initialBoost)
                    boost = interpolateBoost * (1 - (base.fixedAge / initialBoost)) + 1;
                float DistanceToMin = Vector3.Distance(base.transform.position, pos);
                bool tooClose = false;
                direction.y += 5f;
                Vector3 Velocity = base.characterMotor.velocity - (direction.normalized * boost);
                Vector3 difference = (base.transform.position + Velocity) - pos;
                Vector3 OldVelocity = Velocity;
                if (DistanceToMin < minDistance)
                {
                    Log.Debug("Too Close");
                    boost = DistanceToMin / minDistance;
                    Log.Debug(boost);
                    tooClose = true;
                    distance = difference.magnitude;
                }

                if (difference.magnitude > distance)
                {
                    float calculatedDistance = distance + 1;
                    int i = 0;
                    do
                    {
                        Vector3 newPos = base.transform.position + Velocity;
                        float distanceToNew = Vector3.Distance(pos, newPos);
                        float relativeDistance = 0;
                        relativeDistance = distance / distanceToNew;
                        Vector3 newTarget = (relativeDistance * newPos) + ((1 - relativeDistance) * pos);
                        Vector3 newDirection = base.transform.position - newTarget;
                        float oldMag = base.characterMotor.velocity.magnitude;
                        Velocity = (-newDirection.normalized * oldMag);
                        calculatedDistance = Vector3.Distance(base.transform.position + Velocity, pos);
                        i++;
                    } while (calculatedDistance > distance && i < 10);

                }
                else if (difference.magnitude < distance && !tooClose)
                    distance = difference.magnitude;
                Velocity *= boost;
                Vector3 VelP1 = ((Velocity * 0.1f) + (OldVelocity * 0.9f));
                float moveInfluence = 5;
                int moveInfluenceChangeDelay = 1;
                int moveInfluenceDuration = 1;
                if (base.fixedAge >= moveInfluenceChangeDelay)
                    moveInfluence = Mathf.Clamp(
                        1 - ((base.fixedAge - moveInfluenceChangeDelay) / moveInfluenceDuration),
                        0,
                        moveInfluence);
                Vector3 VelP2 = base.inputBank.moveVector.normalized * moveInfluence;
                Vector3 outputVel = VelP1 + VelP2;//(((Velocity * 0.25f) + (OldVelocity * 0.75f)) + (base.inputBank.moveVector * 2f));
                if (outputVel.magnitude > maxVelocity)
                    outputVel = outputVel.normalized * maxVelocity;
                base.characterMotor.velocity = outputVel;
                EffectManager.SimpleEffect(EntityStates.GolemMonster.FireLaser.hitEffectPrefab, pos, new Quaternion(0, 0, 0, 0), true);
            }
            else
                Log.Debug("Usedtobeouter");
                //outer.SetNextStateToMain();
            preVel = base.characterMotor.velocity;

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(base.isAuthority)
                swingGrapple();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
