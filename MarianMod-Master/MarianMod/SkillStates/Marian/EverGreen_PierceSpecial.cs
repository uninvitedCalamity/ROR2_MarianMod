using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using System.Collections.Generic;


namespace MarianMod.SkillStates.EverGreen
{
    class EverGreen_PierceSpecial : BaseSkillState
    {
		public Ray modifiedAimRay;
		public float duration = 0.2f;
		public float distanceTrack = 0;
		public int tick;
		public List<HealthComponent> Friends = new List<HealthComponent>();
		public int healCount;
		public float windup;
		Animator animator;
		Transform modelTransform;
		ChildLocator locator;
		float healStore;
		bool hasHealed = false;
		HealthComponent PreviousVictim = null;
		HealthComponent CurrentVictim = null;

		private GenericSkill overriddenSkill;
		private SkillDef primaryOverride;

        #region This section needs to be moved into another skill

        #endregion
        public override void OnEnter()
        {
            base.OnEnter();
			tick = 0;
			//duration = duration / base.attackSpeedStat;
			windup = duration * 0.25f;

			base.characterBody.SetAimTimer(2f);
			this.animator = base.GetModelAnimator();
			this.modelTransform = base.GetModelTransform();
			this.locator = modelTransform.GetComponent<ChildLocator>();

			float firerate = (1 / windup) * base.attackSpeedStat;

			animator.SetFloat("Firerate", firerate);
			base.PlayAnimation("Gesture, Override", "ShootGun", "Firerate", windup);

			//healStore = PheonixMod.SkillStates.EverGreen.EverGreenGainDrain.AlterStorage(modelTransform.gameObject);
			#region This section needs to be moved into another skill

            #endregion
        }

        public void recoil()
		{
			Log.Debug(this.duration - windup);
			float firerate2 = (1 / (this.duration - windup) * base.attackSpeedStat);
			animator.SetFloat("Firerate", firerate2);
			base.PlayAnimation("Gesture, Override", "BowFire", "Firerate", (this.duration - windup));
		}

        public Vector3 Fire(Vector3 input, float distance, Vector3 Direction)
        {
			this.modifiedAimRay = base.GetAimRay();
			modifiedAimRay.origin = input;
			modifiedAimRay.direction = Direction;
			//this.modifiedAimRay.direction = this.laserDirection;
			Vector3 output = Vector3.zero;
			Transform modelTransform = base.GetModelTransform();
			//Util.PlaySound(EntityStates.GolemMonster.FireLaser.attackSoundString, base.gameObject);
			string text = "MuzzleLaser";

			base.PlayAnimation("Gesture", "FireLaser", "FireLaser.playbackRate", this.duration);
			if (EntityStates.GolemMonster.FireLaser.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(EntityStates.GolemMonster.FireLaser.effectPrefab, base.gameObject, text, false);
			}

			float num = distance;
			Vector3 vector = this.modifiedAimRay.origin + this.modifiedAimRay.direction * num;

			RaycastHit raycastHit;

			if (Physics.Raycast(this.modifiedAimRay, out raycastHit, num, LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask))
			{
				vector = raycastHit.point;
				distanceTrack = distance - raycastHit.distance;
				PreviousVictim = CurrentVictim;
				CurrentVictim = raycastHit.collider.GetComponent<HealthComponent>();
			}
			else
			{
				CurrentVictim = null;
				distanceTrack = distance - num;
			}
			output = vector;
			#region Standard
			/*
            ProjectileManager.instance.FireProjectile(Modules.Projectiles.Napalm,
				vector,
				Quaternion.Euler(raycastHit.normal),//aimRay.direction),
				base.gameObject,
				.2f * base.damageStat,
				0f,
				base.RollCrit(),
				DamageColorIndex.Default,
				null);
			*/
			
			new BlastAttack
			{
				attacker = base.gameObject,
				inflictor = base.gameObject,
				teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
				baseDamage = this.damageStat * 50 * healStore,
				baseForce = 0.5f * 0.2f,
				position = vector,
				radius = 3,
				falloffModel = BlastAttack.FalloffModel.SweetSpot,
				bonusForce = 0.5f * this.modifiedAimRay.direction,
				damageType = DamageType.BlightOnHit,
			}.Fire();
			
			
            #endregion
            #region drain
			/*
			Friends.Add(base.GetComponent<HealthComponent>());
			foreach (TeamComponent tc in (TeamComponent[])GameObject.FindObjectsOfType(typeof(TeamComponent)))
			{
				if (Vector3.Distance(tc.gameObject.transform.position, vector) < 8)
				{
					if (tc.GetComponent<HealthComponent>())
					{
						if (tc.teamIndex != this.GetComponent<TeamComponent>().teamIndex)
						{
							if (NetworkServer.active)
							{
								DamageInfo damageInfo = new DamageInfo();
								damageInfo.damage = 10f * base.damageStat * healStore;
								damageInfo.attacker = base.gameObject;
								damageInfo.procCoefficient = 1;
								damageInfo.position = output;
								damageInfo.crit = base.RollCrit();
								damageInfo.damageType = DamageType.BlightOnHit;
								tc.gameObject.GetComponent<HealthComponent>().TakeDamage(damageInfo);
								GlobalEventManager.instance.OnHitEnemy(damageInfo, tc.gameObject);
								GlobalEventManager.instance.OnHitAll(damageInfo, tc.gameObject);
							}
							healCount++;
						}
						else if(tc.gameObject != base.gameObject)
							Friends.Add(tc.GetComponent<HealthComponent>());
					}
				}
			}

			*/
            #endregion
			/*
            Vector3 origin = this.modifiedAimRay.origin;
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					int childIndex = component.FindChildIndex(text);
					if (EntityStates.GolemMonster.FireLaser.tracerEffectPrefab)
					{
						EffectData effectData = new EffectData
						{
							origin = vector,
							start = this.modifiedAimRay.origin
						};
						effectData.SetChildLocatorTransformReference(base.gameObject, childIndex);
						EffectManager.SpawnEffect(EntityStates.GolemMonster.FireLaser.tracerEffectPrefab, effectData, true);
						EffectManager.SpawnEffect(EntityStates.GolemMonster.FireLaser.hitEffectPrefab, effectData, true);
					}
				}
			}
			*/
			Log.Debug("Output = " + output + " BasAimARay = " + base.GetAimRay().origin + " Input = " + input);
			return output;
		}

		public bool hasfired = false;


        public override void FixedUpdate()
        {
            base.FixedUpdate();
			if (base.isAuthority)
			{
				if (!hasfired)
				{
					Log.Debug("Start-----------------------------------------");
					Vector3 Direction = base.GetAimRay().direction.normalized;
					float distance = 100f;
					Vector3 origin = Fire(base.GetAimRay().origin, distance, Direction);
					Log.Debug("Returned Origin = " + origin);

					if (distanceTrack > 0)
					{
						origin = Fire(origin + Direction, distanceTrack, Direction);
						Log.Debug("Returned Origin = " + origin);
					}
					if (distanceTrack > 0)
					{
						origin = Fire(origin + Direction, distanceTrack, Direction);
						Log.Debug("Returned Origin = " + origin);
					}
					if (distanceTrack > 0)
					{
						origin = Fire(origin + Direction, distanceTrack, Direction);
						Log.Debug("Returned Origin = " + origin);
					}

					#region DrawOnly Ray
					distance = Vector3.Distance(base.GetAimRay().origin, origin);
					this.modifiedAimRay = base.GetAimRay();
					modifiedAimRay.direction = Direction;
					Vector3 output = Vector3.zero;
					Transform modelTransform = base.GetModelTransform();
					string text = "MuzzleLaser";

					base.PlayAnimation("Gesture", "FireLaser", "FireLaser.playbackRate", this.duration);
					if (EntityStates.GolemMonster.FireLaser.effectPrefab)
					{
						EffectManager.SimpleMuzzleFlash(EntityStates.GolemMonster.FireLaser.effectPrefab, base.gameObject, text, false);
					}

					float num = distance;
					Vector3 vector = this.modifiedAimRay.origin + this.modifiedAimRay.direction * num;

					RaycastHit raycastHit;

					if (Physics.Raycast(this.modifiedAimRay, out raycastHit, num, 0))
					{
						vector = raycastHit.point;
						distanceTrack = distance - raycastHit.distance;
						PreviousVictim = CurrentVictim;
						CurrentVictim = raycastHit.collider.GetComponent<HealthComponent>();
					}
					else
					{
						CurrentVictim = null;
						distanceTrack = distance - num;
					}
					output = vector;

					if (modelTransform)
					{
						ChildLocator component = modelTransform.GetComponent<ChildLocator>();
						if (component)
						{
							int childIndex = component.FindChildIndex(text);
							if (EntityStates.GolemMonster.FireLaser.tracerEffectPrefab)
							{
								EffectData effectData = new EffectData
								{
									origin = vector,
									start = this.modifiedAimRay.origin
								};
								effectData.SetChildLocatorTransformReference(base.gameObject, childIndex);
								EffectManager.SpawnEffect(EntityStates.GolemMonster.FireLaser.tracerEffectPrefab, effectData, true);
								EffectManager.SpawnEffect(EntityStates.GolemMonster.FireLaser.hitEffectPrefab, effectData, true);
							}
						}
					}
					#endregion
					Log.Debug("End------------------------------------------");
					hasfired = true;
					Util.PlaySound(EntityStates.GolemMonster.FireLaser.attackSoundString, base.gameObject);
					recoil();
				}
			}
			if (base.fixedAge >= 0.3f && hasfired)
			{
				this.outer.SetNextStateToMain();
			}
        }

        public override void OnExit()
        {
			#region This needs to be moved to another skill

            #endregion
            animator.SetBool("DrawBow", false);
			base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain; 
        }
    }
}
