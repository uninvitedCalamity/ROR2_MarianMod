using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using System.Collections.Generic;


namespace MarianMod.SkillStates
{
	class EverGreen_PierceScatter : BaseSkillState
	{
		public Ray modifiedAimRay;
		public float duration = 0.3f;
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
			duration = duration / base.attackSpeedStat;
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

			new BlastAttack
			{
				attacker = base.gameObject,
				inflictor = base.gameObject,
				teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
				baseDamage = this.damageStat * 10 * healStore,
				baseForce = 0.5f * 0.2f,
				position = vector,
				radius = 3,
				falloffModel = BlastAttack.FalloffModel.SweetSpot,
				bonusForce = 0.5f * this.modifiedAimRay.direction,
				
				damageType = DamageType.BlightOnHit,
			}.Fire();


			#endregion
			#region drain

			#endregion
			Vector3 origin = this.modifiedAimRay.origin;
			/*
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
		float currentCount = 0;

		public void Fire()
		{
			float var = 0.2f;
			if (currentCount == 0)
				var = 0;

			Ray aimRay = base.GetAimRay();
			Vector3 newDir = base.gameObject.transform.position;
			newDir += aimRay.direction;

			newDir += new Vector3(Random.Range(-var, var), Random.Range(-var, var), Random.Range(-var, var));
			newDir = newDir - base.gameObject.transform.position;
			newDir = newDir.normalized;
			float distance = 40f;

			Log.Debug("Start-----------------------------------------");
			Vector3 Direction = base.GetAimRay().direction.normalized;
			Vector3 origin = Fire(base.GetAimRay().origin, distance, newDir);
			Log.Debug("Returned Origin = " + origin);
			if (distanceTrack > 0)
			{
				origin = Fire(origin + Direction, distanceTrack, newDir);
				Log.Debug("Returned Origin = " + origin);
			}
			if (distanceTrack > 0)
			{
				origin = Fire(origin + Direction, distanceTrack, newDir);
				Log.Debug("Returned Origin = " + origin);
			}

			#region DrawOnly Ray
			distance = Vector3.Distance(base.GetAimRay().origin, origin);
			this.modifiedAimRay = base.GetAimRay();
			modifiedAimRay.direction = newDir;
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

			currentCount++;
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				if (!hasfired)
				{
					if (currentCount <= 5)
						this.Fire();
					else if (this.hasfired == false)
					{
						this.hasfired = true;
						Util.PlaySound(EntityStates.GolemMonster.FireLaser.attackSoundString, base.gameObject);
					}

					#region Change this bit


                    #endregion
                    recoil();
				}
			}
			if (base.fixedAge >= duration && hasfired)
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
