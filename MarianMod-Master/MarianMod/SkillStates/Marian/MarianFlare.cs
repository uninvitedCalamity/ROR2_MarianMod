using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using RoR2.Projectile;
using System.Collections.Generic;
using R2API;

namespace MarianMod.SkillStates
{
    class MarianFlare : BaseSkillState
    {
        ChildLocator locator;
        Transform modelTransform;

        float baseDuration = 0.25f;
        float windup = 0.1f;
        float scatter = 0;

        private float duration;
        private float windUp;
        private float fireTime;
        private bool hasFired;
        private Animator animator;
        private bool hasReleased = false;
        GameObject bombPrefab;
        float missileDelay = 0.08f; 
        public float currentCount = 0;
        BullseyeSearch search;
        int missileCount = 5;
        int TargetCount;
        int onTarget = 0;
        Transform[] Targets = new Transform[5];
        Transform Target;
        Ray AimRayCopy;
        static public float DamageCoef = 2.5f;
        float targetRefresh = 0.1f;
        Transform[] indicators = new Transform[5];
        GameObject Camera;
        float Range = 150;

        public override void OnEnter()
        {
            base.OnEnter();
            this.search = new BullseyeSearch();
            this.duration = baseDuration / this.attackSpeedStat;
            this.windUp = windup / base.attackSpeedStat;

            this.fireTime = 0.35f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.animator = base.GetModelAnimator();
            this.modelTransform = base.GetModelTransform();
            this.locator = modelTransform.GetComponent<ChildLocator>();

            //bombPrefab = Modules.Projectiles.stickyFirePrefab;
            bombPrefab = Modules.Projectiles.Missile;

            float firerate = (1 / windUp) * base.attackSpeedStat;
            //animator.SetFloat("Firerate", firerate);

            AimRayCopy = base.GetAimRay();
            GetCurrentTargetInfo();
            missileDelay /= base.attackSpeedStat;
            Camera = GameObject.Find("Main Camera(Clone)");
            //missileCount = (int)(missileCount * Mathf.Clamp(base.attackSpeedStat / 2,1,20));

        }

        public void ScatterFire(Vector3 newDir)
        {

            float var = 0.04f;
            if (currentCount == 0)
                var = 0;

            newDir += new Vector3(Random.Range(-var, var), Random.Range(-var, var), Random.Range(-var, var));
            newDir = newDir.normalized;
            //Log.Debug("Start-----------------------------------------");
            Fire(newDir);
            currentCount++;
            #region DrawOnly Ray
            Transform modelTransform = base.GetModelTransform();
            #endregion

            //Log.Debug("End------------------------------------------");


        }

        public bool upwardSearch(GameObject observing, GameObject parent, bool searching)
        {
            bool ChildFound = false;
            do
            {
                if (observing.transform.parent == parent || observing == parent)
                {
                    ChildFound = true;
                    searching = false;
                }
                else
                {
                    if (observing.transform.parent != null)
                        observing = observing.transform.parent.gameObject;
                    else
                        searching = false;
                }
            } while (searching);
            return ChildFound;
        }

        private void GetCurrentTargetInfo()
        {
            for (int i = 0; i < Targets.Length; i++)
            {
                if (Targets[i] != null)
                {
                    Targets[i] = null;
                }
            }
            for (int i = 0; i < indicators.Length; i++)
            {
                if (indicators[i] != null)
                {
                    Destroy(indicators[i].gameObject);
                }
                indicators[i] = null;
            }

            Ray aimRay = base.GetAimRay();
            this.search.filterByDistinctEntity = true;
            this.search.filterByLoS = true;
            this.search.minDistanceFilter = 0f;
            this.search.maxDistanceFilter = Range;//Paint.maxDistance;
            this.search.minAngleFilter = 0f;
            this.search.maxAngleFilter = 45;//Paint.maxAngle;
            this.search.viewer = base.characterBody;
            this.search.searchOrigin = aimRay.origin;
            this.search.searchDirection = aimRay.direction;
            this.search.sortMode = BullseyeSearch.SortMode.Angle;
            this.search.teamMaskFilter = TeamMask.GetUnprotectedTeams(base.GetTeam());
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(base.gameObject);
            //currentTargetHurtBox = null;
            //currentTargetHealthComponent = null;
            foreach (HurtBox hurtBox in this.search.GetResults())
            {
                if (hurtBox.healthComponent && hurtBox.healthComponent.alive)
                {
                    if (TargetCount >= missileCount)
                        return;

                    bool interrupted = false;
                    RaycastHit raycastHit = new RaycastHit();
                    Ray BaseRay = base.GetAimRay();
                    float num = Vector3.Distance(BaseRay.origin, hurtBox.transform.position);
                    BaseRay.direction = BaseRay.origin - hurtBox.transform.position;
                    BaseRay.direction = -BaseRay.direction;
                    if (Physics.Raycast(BaseRay, out raycastHit, num, LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask))
                    {
                        HurtBox h;
                        GameObject parent = hurtBox.healthComponent.gameObject;
                        bool ChildFound = false;
                        GameObject observing = raycastHit.collider.gameObject;
                        bool searching = true;
                        if (h = raycastHit.collider.GetComponent<HurtBox>())
                        {
                            if (h.healthComponent == hurtBox.healthComponent)
                            {
                                ChildFound = true;
                                searching = false;
                            }
                                
                        }
                        do 
                        {
                            if (observing.transform.parent == parent || observing == parent)
                            {
                                ChildFound = true;
                                searching = false;
                            }
                            else
                            {
                                if (observing.transform.parent != null)
                                    observing = observing.transform.parent.gameObject;
                                else
                                    searching = false;
                            }
                        } while (searching);

                        interrupted = !ChildFound;
                    }

                    BaseRay.origin = locator.FindChild("MissilePoint").position;
                    num = Vector3.Distance(BaseRay.origin, hurtBox.transform.position);
                    BaseRay.direction = BaseRay.origin - hurtBox.transform.position;
                    BaseRay.direction = -BaseRay.direction;

                    if (interrupted)
                    {
                        if (Physics.Raycast(BaseRay, out raycastHit, num, LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask))
                        {
                            HurtBox h;
                            GameObject parent = hurtBox.healthComponent.gameObject;
                            bool ChildFound = false;
                            GameObject observing = raycastHit.collider.gameObject;
                            bool searching = true;
                            if (h = raycastHit.collider.GetComponent<HurtBox>())
                            {
                                if (h.healthComponent == hurtBox.healthComponent)
                                {
                                    ChildFound = true;
                                    searching = false;
                                }

                            }
                            do
                            {
                                if (observing.transform.parent == parent || observing == parent)
                                {
                                    ChildFound = true;
                                    searching = false;
                                }
                                else
                                {
                                    if (observing.transform.parent != null)
                                        observing = observing.transform.parent.gameObject;
                                    else
                                        searching = false;
                                }
                            } while (searching);

                            interrupted = !ChildFound;
                        }
                    }

                    if (!interrupted)
                    {
                        Targets[TargetCount] = hurtBox.transform;

                        if (base.isAuthority)
                        {
                            if (indicators[TargetCount] == null)
                            {
                                Quaternion LR = Quaternion.LookRotation((hurtBox.transform.position - BaseRay.origin).normalized);
                                indicators[TargetCount] = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.MissileSprite, hurtBox.transform.position, LR).transform;
                                indicators[TargetCount].localScale = new Vector3(1, 1, 1);
                                indicators[TargetCount].LookAt(BaseRay.origin);
                            }
                        }
                        TargetCount++;                      
                    }
                }
            }
        }


        public Vector3 setPostion(Transform TargetObject, Vector3 FarPosition)
        {           
            if (Camera != null)
            {
                float Distance = Vector3.Distance(FarPosition, Camera.transform.position);
                float percent = 0.1f / Distance;
                Vector3 currentTargetObject = (Camera.transform.position * (1 - percent)) + (FarPosition * (percent));
                if (TargetObject != null)
                {
                    SpriteRenderer renderer = TargetObject.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
                    renderer.material.color = Color.HSVToRGB(0.3f, 1, 1);
                    TargetObject.position = currentTargetObject;
                    TargetObject.LookAt(Camera.transform.position);
                    TargetObject.localScale = new Vector3(0.002f, 0.002f, 0.002f) / Mathf.Clamp(Distance* 0.01f,0.1f,Range);
                }
            }
            else
                Log.Debug("Camera is Null");
            return Vector3.zero;
        }
        public override void Update()
        {
            base.Update();

            for (int i = 0; i < Targets.Length; i++)
            {
                if (Targets[i] != null)
                {
                    if (indicators[i] != null)
                    {
                        setPostion(indicators[i], Targets[i].transform.position);
                    }
                }
            }
        }
        bool Refunded = false;
        public void Fire(Vector3 newDir)
        {
            GameObject Missile = Modules.Projectiles.Missile;
            base.PlayAnimation("Gesture, Override", "ShootGun", "Firerate", windup);
            if (Targets[onTarget] != null)
            {
                Target = Targets[onTarget];// nijhoqefw.transform;
                onTarget++;
            }
            if (onTarget >= TargetCount)
                onTarget = 0;

            if (Target != null)
            {
                MissileUtils.FireMissile(
                    locator.FindChild("MissilePoint").position,
                    base.characterBody,
                    default(ProcChainMask),
                    Target.gameObject,
                    this.damageStat * DamageCoef,
                    base.RollCrit(),
                    Missile,
                    DamageColorIndex.Default,
                    newDir,
                    0f,
                    false);

                string text = "MissilePoint";

                if (EntityStates.GolemMonster.FireLaser.effectPrefab)
                {
                    EffectManager.SimpleMuzzleFlash(EntityStates.Engi.EngiWeapon.FireGrenades.effectPrefab, base.gameObject, text, true);
                }
            }
            else
            {
                if (!Refunded)
                {
                    Refunded = true;
                    if (base.activatorSkillSlot.stock < base.activatorSkillSlot.maxStock)
                        base.activatorSkillSlot.AddOneStock();
                    EffectManager.SimpleMuzzleFlash(EntityStates.EngiTurret.EngiTurretWeapon.FireGauss.effectPrefab, base.gameObject, "MissilePoint", true);
                }
            }
            /*Vector3 initialDirection, float force, bool addMissileProc)
            ProjectileManager.instance.FireProjectile(Missile,
                locator.FindChild("MissilePoint").position,
                Util.QuaternionSafeLookRotation(newDir),//aimRay.direction),
                base.gameObject,
                4f * base.damageStat,
                0f,
                base.RollCrit(),
                DamageColorIndex.Default,
                null);*/

        }
        float counter = 0;
        float timer = 0;
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterBody.SetAimTimer(2f);
            for (int i = 0; i < indicators.Length; i++)
            {
                if (indicators[i] != null)
                {
                    indicators[i].LookAt(base.GetAimRay().origin);
                    if (Targets[i] != null)
                        indicators[i].position = Targets[i].position;
                }
            }
            if (timer >= targetRefresh && !hasReleased)
            {
                TargetCount = 0;
                GetCurrentTargetInfo();
                timer = 0;
            }
            timer += Time.fixedDeltaTime;
            if (base.inputBank.skill2.justReleased && !hasReleased)
                hasReleased = true;
            if (base.isAuthority)
            {
                if (base.fixedAge >= windup)
                {                    
                    if (hasReleased)
                    {
                        //Log.Debug("In Firing Cycle");
                        //Util.PlaySound("PheonixBombThrow", base.gameObject);

                        Ray aimRay = base.GetAimRay();
                        Vector3 newDir = base.gameObject.transform.position;
                        newDir += aimRay.direction;
                        //newDir += Vector3.up / 20;
                        scatter = 0;

                        newDir += new Vector3(Random.Range(-scatter, scatter), Random.Range(-scatter, scatter), Random.Range(-scatter, scatter));
                        newDir = newDir - base.gameObject.transform.position;
                        newDir = newDir.normalized;

                        if (counter >= missileDelay)
                        {
                            if (currentCount < missileCount)
                                ScatterFire(newDir);
                            counter = 0;
                        }

                        counter += Time.fixedDeltaTime;
                    }
                }                    
                if (base.fixedAge >= duration && currentCount >= missileCount)
                    outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnExit()
        {
            for (int i = 0; i < Targets.Length; i++)
            {
                if (Targets[i] != null)
                {
                    Targets[i] = null;
                }
            }
            for (int i = 0; i < indicators.Length; i++)
            {
                if (indicators[i] != null)
                {
                    Destroy(indicators[i].gameObject);
                }
                indicators[i] = null;
            }
            base.OnExit();
        }
    }
}
