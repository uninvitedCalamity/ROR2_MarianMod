using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using RoR2.Projectile;
using System.Collections.Generic;

namespace MarianMod.SkillStates
{
    class PrimaryFire :BaseSkillState
    {
        ChildLocator locator;
        Transform modelTransform;

        float baseDuration = 0.25f;
        float windup = 0.01f;
        float scatter = 0;

        private float duration;
        private float windUp;
        private float fireTime;
        private bool hasFired;
        private Animator animator;
        private bool hasReleased = false;
        public float currentCount = 0;
        float Range = 50;
        GameObject impactFlash;
        int projectileCount = 5;
        static public float DamageCoef = 1;
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = baseDuration / this.attackSpeedStat;
            this.windUp = windup / base.attackSpeedStat;

            this.fireTime = 0.35f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.animator = base.GetModelAnimator();
            this.modelTransform = base.GetModelTransform();
            this.locator = modelTransform.GetComponent<ChildLocator>();

            base.PlayAnimation("Gesture, Override", "ShootGun", "ShootGun.playbackRate", this.duration);
            float firerate = (1 / windUp) * base.attackSpeedStat;
            //animator.SetFloat("Firerate", firerate);
            base.PlayAnimation("Gesture, Override", "ShootGun", "Firerate", windup);

            impactFlash = EntityStates.Commando.CommandoWeapon.FireBarrage.hitEffectPrefab;
            string text = "MissilePoint";

            if (EntityStates.GolemMonster.FireLaser.effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(EntityStates.Engi.EngiWeapon.FireGrenades.effectPrefab, base.gameObject, text, false);
            }
        }

        public void ScatterFire(Vector3 newDir)
        {

            float var = 0.05f;
            if (currentCount == 0)
                var = 0;
            currentCount++;


            newDir += new Vector3(Random.Range(-var, var), Random.Range(-var, var), Random.Range(-var, var));
            newDir = newDir.normalized;
            //Log.Debug("Start-----------------------------------------");
            //Fire(newDir);
            Fire2(/*locator.FindChild("FirePoint").position*/base.GetAimRay().origin, Range, newDir);
            #region DrawOnly Ray
            Transform modelTransform = base.GetModelTransform();
            #endregion

            //Log.Debug("End------------------------------------------");


        }

        public void Fire2(Vector3 ElectroOutput, float distance, Vector3 Direction)
        {
            Vector3 AimDirection = -Direction;

            new BulletAttack
            {
                bulletCount = 1,
                aimVector = -AimDirection,//aimRay.direction,
                origin = ElectroOutput,
                damage = DamageCoef * this.damageStat,
                damageColorIndex = DamageColorIndex.Default,
                damageType = DamageType.Generic,
                falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                maxDistance = distance,
                force = 1,
                hitMask = LayerIndex.CommonMasks.bullet,
                minSpread = 0f,
                maxSpread = 0f,
                isCrit = base.RollCrit(),
                owner = base.gameObject,
                muzzleName = "MissilePoint",
                smartCollision = false,
                procChainMask = default(ProcChainMask),
                procCoefficient = 1 / (float)projectileCount,
                radius = 0.75f,
                sniper = false,
                stopperMask = LayerIndex.CommonMasks.bullet,
                weapon = null,
                tracerEffectPrefab = EntityStates.Commando.CommandoWeapon.FireShrapnel.tracerEffectPrefab,//Shoot.tracerEffectPrefab,
                spreadPitchScale = 0f,
                spreadYawScale = 0f,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                hitEffectPrefab = impactFlash//EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
            }.Fire();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= windup)
                if (!hasFired)
                {
                    this.hasFired = true;
                    Util.PlaySound(EntityStates.Commando.CommandoWeapon.FirePistol2.firePistolSoundString, base.gameObject);
                    if (!base.isAuthority)
                        return;
                    hasReleased = true;

                    Ray aimRay = base.GetAimRay();
                    Vector3 anglePoint = aimRay.origin;
                    Vector3 newDir = anglePoint;
                    newDir += aimRay.direction;
                    scatter = 0;

                    newDir += new Vector3(Random.Range(-scatter, scatter), Random.Range(-scatter, scatter), Random.Range(-scatter, scatter));
                    newDir = newDir - anglePoint;
                    newDir = newDir.normalized;
                    do
                    { ScatterFire(newDir); }
                    while (currentCount < projectileCount);

                }
            if (!base.isAuthority)
                return;
            if (base.fixedAge >= duration && hasFired)
                outer.SetNextStateToMain();
            
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
