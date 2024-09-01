using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using RoR2.Projectile;
using System.Collections.Generic;
using R2API;
using UnityEngine.Networking;

namespace MarianMod.SkillStates
{
    /*
     * 			base.DoImpactAuthority();
			BlastAttack.Result result = base.DetonateAuthority();
			base.skillLocator.utility.RunRecharge((float)result.hitCount * ChainableLeap.refundPerHit);
    */

    class MarianFlare : BaseSkillState
    {
        ChildLocator locator;
        Transform modelTransform;

        float baseDuration = 0.25f;
        float coolDown = 0.8f;
        float windup = 0.1f;
        float scatter = 0;

        private float duration;
        private float windUp;
        private float fireTime;
        private bool hasFired = false;
        private Animator animator;
        private bool hasReleased = false;
        GameObject bombPrefab;
        float missileDelay = 0.055f;
        public float currentCount = 0;
        BullseyeSearch search;
        int missileCount = 5;
        int TargetCount;
        int onTarget = 0;
        //Transform[] Targets = new Transform[50];
        Transform Target;
        Ray AimRayCopy;
        static public float DamageCoef = 2.5f;
        float targetRefresh = 0.1f;
        //Transform[] indicators = new Transform[50];
        GameObject Camera;
        public const float Range = 150;
        float IterDelay = .9f;
        float IterTimer = 0;
        int charge = 1;
        int pingCount = 0;
        GameObject Ping;
        GameObject Ping2;
        GameObject Ping3;
        bool FinalPing = false;
        bool CoolingDown = false;
        int firingcount = 0;
        GameObject Missile;
        MarianMod.Modules.CustomScripts.SpriteManager SM;

        public override void OnEnter()
        {
            base.OnEnter();
            this.search = new BullseyeSearch();
            this.duration = baseDuration / this.attackSpeedStat;
            this.windUp = windup / base.attackSpeedStat;
            missileDelay /= base.attackSpeedStat;
            coolDown /= base.attackSpeedStat;
            counter = missileDelay;

            this.fireTime = 0.35f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.animator = base.GetModelAnimator();
            this.modelTransform = base.GetModelTransform();
            this.locator = modelTransform.GetComponent<ChildLocator>();
            SM = characterBody.GetComponent<MarianMod.Modules.CustomScripts.SpriteManager>();
            //bombPrefab = Modules.Projectiles.stickyFirePrefab;
            bombPrefab = Modules.Projectiles.Missile;

            float firerate = (1 / windUp) * base.attackSpeedStat;
            //animator.SetFloat("Firerate", firerate);

            AimRayCopy = base.GetAimRay();
            GetCurrentTargetInfo();

            Camera = GameObject.Find("Main Camera(Clone)");

            Ping = Modules.Assets.MissileChargePing;
            Ping2 = Modules.Assets.MissileChargePing2;
            Ping3 = Modules.Assets.MissileChargePing3;
            Missile = setProj();
            //missileCount = (int)(missileCount * Mathf.Clamp(base.attackSpeedStat / 2,1,20));

        }

        public virtual GameObject setProj()
        {
            return Modules.Projectiles.Missile;
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
            for (int i = 0; i < SM.Targets.Length; i++)
            {
                if (SM.Targets[i] != null)
                {
                    SM.Targets[i] = null;
                }
            }
            for (int i = 0; i < SM.indicators.Length; i++)
            {
                if (SM.indicators[i] != null)
                {
                    Destroy(SM.indicators[i].gameObject);
                }
                SM.indicators[i] = null;
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
                        //Move this to script on Model, like with mongrel's old Borzoi
                        SM.Targets[TargetCount] = hurtBox.transform;

                        if (base.isAuthority && base.characterBody.isPlayerControlled)
                        {
                            if (SM.indicators[TargetCount] == null)
                            {
                                Quaternion LR = Quaternion.LookRotation((hurtBox.transform.position - BaseRay.origin).normalized);
                                SM.indicators[TargetCount] = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.MissileSprite, hurtBox.transform.position, LR).transform;
                                SM.indicators[TargetCount].localScale = new Vector3(1, 1, 1);
                                SM.indicators[TargetCount].LookAt(BaseRay.origin);
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

                Vector3 Difference = Camera.transform.position - FarPosition;
                Vector3 Direction = Difference.normalized;
                Vector3 MaxSize = new Vector3(0.002f, 0.002f, 0.002f) * base.GetComponent<MarianMod.Modules.CharacterDataStore>().UIScale;
                Vector3 MinSize = new Vector3(0.001f, 0.001f, 0.001f) * base.GetComponent<MarianMod.Modules.CharacterDataStore>().UIScale;
                float RangePercent = Distance / Range;

                if (TargetObject != null)
                {
                    SpriteRenderer renderer = TargetObject.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
                    renderer.material.color = Color.HSVToRGB(0.3f, 1, 1);
                    TargetObject.position = currentTargetObject;
                    //TargetObject.position = FarPosition;
                    TargetObject.LookAt(Camera.transform.position);
                    //TargetObject.localScale = new Vector3(0.0005f, 0.0005f, 0.0005f) / Mathf.Clamp(Distance* 0.01f,0.2f,Range);
                    Vector3 L = MaxSize * (1 - RangePercent);
                    Vector3 M = MinSize * RangePercent;
                    TargetObject.localScale = L + M;
                }
            }
            else
                Log.Debug("Camera is Null");
            return Vector3.zero;
        }

        public void UpdateIndicators()
        {
            for (int i = 0; i < SM.Targets.Length; i++)
            {
                if (SM.Targets[i] != null)
                {
                    if (SM.indicators[i] != null)
                    {
                        setPostion(SM.indicators[i], SM.Targets[i].transform.position);
                    }
                }
            }
        }

        public override void Update()
        {
            base.Update();
            //UpdateIndicators();
        }
        bool Refunded = false;
        public void Fire(Vector3 newDir)
        {
            base.PlayAnimation("Gesture, Override", "ShootGun", "Firerate", windup);
            if (SM.Targets[onTarget] != null)
            {
                Target = SM.Targets[onTarget];// nijhoqefw.transform;
                onTarget++;
            }
            else
            {
            }
            if (onTarget >= TargetCount)
                onTarget = 0;

            if (Target != null)
            {
                MissileUtils.FireMissile(
                    locator.FindChild("FirePoint").position,
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

                string text = "FirePoint";

                if (EntityStates.Engi.EngiWeapon.FireGrenades.effectPrefab)
                {
                    EffectManager.SimpleMuzzleFlash(EntityStates.Engi.EngiWeapon.FireGrenades.effectPrefab, base.gameObject, text, true);
                }
                currentCount++;
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
        }
        float counter = 0;
        float timer = 0;
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            //UpdateIndicators();
            base.characterBody.SetAimTimer(2f);
            if (!base.inputBank.skill2.down && !hasReleased)
                hasReleased = true;
            if (base.isAuthority)
            {
                for (int i = 0; i < SM.indicators.Length; i++)
                {
                    if (SM.indicators[i] != null)
                    {
                        SM.indicators[i].LookAt(base.GetAimRay().origin);
                        if (SM.Targets[i] != null)
                            SM.indicators[i].position = SM.Targets[i].position;
                    }
                }
                if (base.fixedAge >= windup)
                {
                    if (hasReleased && !CoolingDown)
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


                        if (counter >= missileDelay && !Refunded)
                        {
                            if (currentCount < missileCount)
                            {
                                ScatterFire(newDir);
                                ProjectileManager.instance.FireProjectile(Modules.Projectiles.AddMissiles,
                                    locator.FindChild("FirePoint").position,
                                    Util.QuaternionSafeLookRotation(Vector3.zero),//aimRay.direction),
                                    base.gameObject,
                                    missileCount - currentCount,
                                    0f,
                                    false,
                                    DamageColorIndex.Default,
                                    null);
                            }
                            if (!Refunded)
                            {
                                counter = 0;
                                hasFired = true;
                            }
                        }
                        

                        counter += Time.fixedDeltaTime;
                    }
                }
                if (!CoolingDown && (currentCount < missileCount && !Refunded))
                {
                    duration = base.fixedAge + coolDown;
                }
                if (!CoolingDown)
                {
                    if (currentCount >= missileCount)
                    {
                        CoolingDown = true;
                    }
                    if (!hasReleased)
                    {
                        if (base.fixedAge >= windup)
                        {

                            if (IterTimer >= IterDelay / base.attackSpeedStat && TargetCount >= 1)
                            {
                                if (charge < 3)
                                {
                                    pingCount = 0;
                                    if (base.isAuthority && !CoolingDown)
                                        missileCount = Mathf.Clamp(missileCount + 3, 5, SM.Targets.Length);
                                }
                                IterTimer = 0;
                                charge += 1;
                            }
                            else if (TargetCount <= 0)
                            {
                                missileCount = 5;
                                charge = 1;
                                pingCount = 0;
                                IterTimer = 0;
                                FinalPing = false;
                            }
                            IterTimer += Time.fixedDeltaTime;

                        }
                        if (pingCount < charge && TargetCount > 0 && !FinalPing)
                        {
                            pingCount = charge;
                            if (charge == 3)
                            {
                                FinalPing = true;
                                EffectManager.SimpleEffect(Ping3, locator.FindChild("MissilePoint").transform.position, new Quaternion(0, 0, 0, 0), true);
                            }
                            else if (charge == 2)
                                EffectManager.SimpleEffect(Ping2, locator.FindChild("MissilePoint").transform.position, new Quaternion(0, 0, 0, 0), true);
                            else
                                EffectManager.SimpleEffect(Ping, locator.FindChild("MissilePoint").transform.position, new Quaternion(0, 0, 0, 0), true);
                        }
                    }

                }//!CoolingDown end
                firingcount = (int)(missileCount - currentCount);
                ProjectileManager.instance.FireProjectile(Modules.Projectiles.AddMissiles,
                    Vector3.down * 100,
                    Util.QuaternionSafeLookRotation(Vector3.down * 100),//aimRay.direction),
                    base.gameObject,
                    firingcount,
                    0f,
                    false,
                    DamageColorIndex.Default,
                    null);
                if (base.fixedAge >= duration)
                {
                    outer.SetNextStateToMain();
                    return;
                }
                if (timer >= targetRefresh && (!hasReleased))
                {
                    TargetCount = 0;
                    GetCurrentTargetInfo();
                    timer = 0;
                }
                timer += Time.fixedDeltaTime;
            }

            #region Buffs and Pings
            #endregion
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnExit()
        {
            for (int i = 0; i < SM.Targets.Length; i++)
            {
                if (SM.Targets[i] != null)
                {
                    SM.Targets[i] = null;
                }
            }
            for (int i = 0; i < SM.indicators.Length; i++)
            {
                if (SM.indicators[i] != null)
                {
                    Destroy(SM.indicators[i].gameObject);
                }
                SM.indicators[i] = null;
            }
            if(base.isAuthority && !hasFired)
                if (base.activatorSkillSlot.stock < base.activatorSkillSlot.maxStock)
                    base.activatorSkillSlot.AddOneStock();
            base.characterBody.ClearTimedBuffs(Modules.Buffs.MissileBuff);
            base.OnExit();
        }
    }
}
