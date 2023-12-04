using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using System.Collections.Generic;

namespace MarianMod.SkillStates
{
    class Marian_ShatteringPierce : BaseSkillState
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

		public bool hasfired = false;
		int currentCount = 0;
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
        }

		public Vector3 Fire(Vector3 input, float distance, Vector3 Direction)
		{
			modifiedAimRay = base.GetAimRay();
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
				if (raycastHit.collider.GetComponent<HealthComponent>())
				{
					PreviousVictim = CurrentVictim;
					CurrentVictim = raycastHit.collider.GetComponent<HealthComponent>();
				}
				//Log.Debug(CurrentVictim.gameObject.name);
				if (currentCount == 0)
				{
					do
					{
						this.ScatterFire(raycastHit.point, this.modifiedAimRay.direction); 
					}
					while (currentCount < 5);
				}
			}
			else
			{
				CurrentVictim = null;
				distanceTrack = distance - num;
			}
			output = vector;

			new BlastAttack
			{
				attacker = base.gameObject,
				inflictor = base.gameObject,
				teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
				baseDamage = this.damageStat,
				baseForce = 0.5f * 0.2f,
				position = vector,
				radius = 3,
				falloffModel = BlastAttack.FalloffModel.SweetSpot,
				bonusForce = 0.5f * this.modifiedAimRay.direction,
				damageType = DamageType.Freeze2s,
			}.Fire();
			//Log.Debug("Output = " + output + " BasAimARay = " + base.GetAimRay().origin + " Input = " + input);
			return output;
		}


		public void recoil()
		{
			Log.Debug(this.duration - windup);
			float firerate2 = (1 / (this.duration - windup) * base.attackSpeedStat);
			animator.SetFloat("Firerate", firerate2);
			base.PlayAnimation("Gesture, Override", "BowFire", "Firerate", (this.duration - windup));
		}

		public void spawnLaser(string FPoint, ChildLocator component, Vector3 target, Vector3 V, Vector3 StartVector, GameObject LaserCopy1)
		{
			Transform ElectroOutput = component.FindChild(FPoint);
			GameObject LaserCopy = LaserCopy1;
			LaserCopy.transform.position = StartVector;
			float velocity = V.z;
			ParticleSystem.MainModule main = LaserCopy.GetComponent<ParticleSystem>().main;
			main.startSpeed = 25 * velocity;
			V.z = 1;
			LaserCopy.transform.localScale = V;
			LaserCopy.transform.LookAt(target);
			EffectManager.SimpleEffect(LaserCopy, LaserCopy.transform.position, LaserCopy.transform.rotation, true);
		}

		public void ScatterFire(Vector3 origin, Vector3 newDir)
		{
			Vector3 startOrigin = origin;
			float var = 0.1f;
			if (currentCount == 0)
				var = 0;
			currentCount++;


			newDir += new Vector3(Random.Range(-var, var), Random.Range(-var, var), Random.Range(-var, var));
			newDir = newDir.normalized;
			float distance = 600f;

			distanceTrack = 0;
			Log.Debug("Start-----------------------------------------");
			origin = Fire(origin, distance, newDir);
			Log.Debug("Returned Origin = " + origin);
			if (distanceTrack > 0)
			{
				origin = Fire(origin + newDir, distanceTrack, newDir);
				Log.Debug("Returned Origin = " + origin);
			}
			if (distanceTrack > 0)
			{
				origin = Fire(origin + newDir, distanceTrack, newDir);
				Log.Debug("Returned Origin = " + origin);
			}

			#region DrawOnly Ray
			Transform modelTransform = base.GetModelTransform();
			string text = "MuzzleLaser";

			if (EntityStates.GolemMonster.FireLaser.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(EntityStates.GolemMonster.FireLaser.effectPrefab, base.gameObject, text, false);
			}


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
							origin = startOrigin,
							start = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100))
						};						
						effectData.SetChildLocatorTransformReference(base.gameObject, childIndex);
						effectData.rotation = Quaternion.LookRotation(newDir);
						Vector3 V = new Vector3(1, 1, Vector3.Distance(startOrigin, origin) / 10);
						Log.Debug("Calling Spawn laser");
						spawnLaser("FirePoint", locator, origin, V, startOrigin, Modules.Assets.OverchargeShot2);
						//EffectManager.SpawnEffect(EntityStates.GolemMonster.FireLaser.tracerEffectPrefab, effectData, true);
						//EffectManager.SpawnEffect(EntityStates.GolemMonster.FireLaser.hitEffectPrefab, effectData, true);
					}
				}
			}
			#endregion

			Log.Debug("End------------------------------------------");


		}
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

					/*
					if (distanceTrack > 0)
					{
						origin = Fire(origin + Direction, distanceTrack, Direction);
						Log.Debug("Returned Origin = " + origin);
					}
					*/

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
								//EffectManager.SpawnEffect(EntityStates.GolemMonster.FireLaser.tracerEffectPrefab, effectData, true);
								Vector3 V = new Vector3(1, 1, Vector3.Distance(modifiedAimRay.origin, origin) / 10);
								Log.Debug("Calling Spawn laser");
								Transform firepoint = locator.FindChild("FirePoint");
								spawnLaser("FirePoint", locator, origin, V, firepoint.position, Modules.Assets.OverchargeShot2);
								//EffectManager.SpawnEffect(EntityStates.GolemMonster.FireLaser.hitEffectPrefab, effectData, true);
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
            base.OnExit();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
			return InterruptPriority.Death;
        }
    }
}
