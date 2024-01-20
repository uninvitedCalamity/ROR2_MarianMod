using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using RoR2.Projectile;
using System.Collections.Generic;
using System;

namespace MarianMod.SkillStates
{
    class MarianIceBomb : BaseSkillState
    {
        ChildLocator locator;
        Transform modelTransform;

        float baseDuration = 0.25f;
        float windup = 0.2f;
        float scatter = 0;

        private float duration;
        private float windUp;
        private bool hasFired;
        private Animator animator;
        private bool hasReleased = false;
        GameObject Icebomb;
        public float currentCount = 0;
        float base_desired_speed = 10;
        float max_multiplier = 500;
        static public float DamageCoef = 0.1f;
        public float damage_mult = 1;
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = baseDuration;
            this.windUp = windup;

            base.characterBody.SetAimTimer(2f);
            this.animator = base.GetModelAnimator();
            this.modelTransform = base.GetModelTransform();
            this.locator = modelTransform.GetComponent<ChildLocator>();

            Icebomb = setPrefab();

            //base.PlayAnimation("Gesture, Override", "ShootGun", "ShootGun.playbackRate", this.duration);
            float firerate = (1 / windUp) * base.attackSpeedStat;
            //animator.SetFloat("Firerate", firerate);
            //base.PlayAnimation("Gesture, Override", "ShootGun", "Firerate", windup);
            damage_mult = setMult();
        }

        public virtual float setMult()
        {
            return 1;
        }

        public virtual GameObject setPrefab()
        {
            return Modules.Projectiles.IceBomb;
        }

        public void ScatterFire(Vector3 newDir)
        {

            float var = 0.08f;
            if (currentCount == 0)
                var = 0;
            currentCount++;


            newDir += new Vector3(UnityEngine.Random.Range(-var, var), UnityEngine.Random.Range(-var, var), UnityEngine.Random.Range(-var, var));
            newDir = newDir.normalized;
            //Log.Debug("Start-----------------------------------------");
            this.Fire(newDir);
            #region DrawOnly Ray
            Transform modelTransform = base.GetModelTransform();
            #endregion

            //Log.Debug("End------------------------------------------");


        }

        public void Fire(Vector3 newDir)
        {
            try
            {
                //bombPrefab.GetComponent<ProjectileSimple>().desiredForwardSpeed = base_desired_speed;
                ProjectileManager.instance.FireProjectile(this.Icebomb,
                    locator.FindChild("FirePoint").position,
                    Util.QuaternionSafeLookRotation(newDir),//aimRay.direction),
                    base.gameObject,
                    DamageCoef * base.damageStat * damage_mult,
                    0f,
                    base.RollCrit(),
                    DamageColorIndex.Default,
                    null);
            }
            catch (NullReferenceException e)
            {
                Log.Debug(e);
            }
        }

        public void StartFire()
        {
            if (!hasFired)
            {
                hasReleased = true;
                this.hasFired = true;
                //Util.PlaySound("PheonixBombThrow", base.gameObject);
                base_desired_speed *= (Mathf.Clamp(base.fixedAge / windup, 0, max_multiplier));
                Ray aimRay = base.GetAimRay();
                Vector3 newDir = base.gameObject.transform.position;
                newDir.y += 0.1f;
                newDir += aimRay.direction;
                //newDir += Vector3.up / 20;
                scatter = 0;

                newDir += new Vector3(UnityEngine.Random.Range(-scatter, scatter), UnityEngine.Random.Range(-scatter, scatter), UnityEngine.Random.Range(-scatter, scatter));
                newDir = newDir - base.gameObject.transform.position;
                newDir = newDir.normalized;
                if (!base.characterMotor.isGrounded)
                    base.characterMotor.velocity += -newDir * Mathf.Clamp(base.fixedAge / windup, 0, max_multiplier);

                this.ScatterFire(newDir);
            }
        }
        bool released = false;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= windUp && !hasFired)
            {
                base.PlayAnimation("Gesture, Override", "ThrowBomb");
                if(!base.isAuthority)
                    hasFired = true;
            }
            if (base.isAuthority)
            {
                if(!hasFired && base.fixedAge >= windup)
                    this.StartFire();
                if (base.fixedAge >= duration && hasFired)
                    outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
