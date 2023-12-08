using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace MarianMod.Modules
{
    internal static class Projectiles
    {
        internal static GameObject boltPrefab;
        internal static GameObject stickyFirePrefab;
        internal static GameObject NapalmLarge;
        internal static GameObject IceBomb;
        internal static GameObject Missile;


        public static float iceProc = 2.3f;

        internal static void RegisterProjectiles()
        {
            CreateIceBomb();
            Missile = CreateMissile(0);

            AddProjectile(Missile);
            AddProjectile(IceBomb);
        }

        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Modules.Content.AddProjectilePrefab(projectileToAdd);
        }

        private static GameObject CreateMissile(int No)
        {
            Log.Debug("Create Missile");
            GameObject Missile = CloneProjectilePrefab("EngiHarpoon", "MarianMissile");
            Log.Debug("cloned");
            Missile.GetComponent<ProjectileDamage>().damageType = DamageType.BlightOnHit;
            Log.Debug("Damage Type set");
            ProjectileSingleTargetImpact bombImpactExplosion = Missile.GetComponent<ProjectileSingleTargetImpact>();
            Log.Debug("Grab Impact Explosion");
            bombImpactExplosion.impactEffect = Modules.Assets.marianEnergyProj;
            Log.Debug("Assign explosionEffect");
            ProjectileController bombController = Missile.GetComponent<ProjectileController>();
            Log.Debug("Get ProjectileController");
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("MarianMissile2") != null) bombController.ghostPrefab = CreateGhostPrefab("MarianMissile2");
            Log.Debug("Assign Model");
            MissileController mc = Missile.GetComponent<MissileController>();
            mc.turbulence = 0.75f;
            mc.acceleration = 5;
            return Missile;
        }

        private static void CreateIceBomb()
        {
            IceBomb = CloneProjectilePrefab("CommandoGrenadeProjectile", "MarianIceBombProjectile");

            ProjectileImpactExplosion bombImpactExplosion = IceBomb.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(bombImpactExplosion);
            ProjectileDamage pd = IceBomb.GetComponent<ProjectileDamage>();
            ProjectileSimple ps = IceBomb.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 50;
            pd.damageType = DamageType.Freeze2s;
            

            bombImpactExplosion.blastRadius = 10 * 2f;
            bombImpactExplosion.destroyOnEnemy = true;
            bombImpactExplosion.destroyOnWorld = true;
            bombImpactExplosion.lifetime = 6f;
            bombImpactExplosion.impactEffect = Modules.Assets.bombExplosionEffect;
            bombImpactExplosion.blastProcCoefficient = iceProc;
            //bombImpactExplosion.explosionEffect = bombImpactExplosion.impactEffect;
            //bombImpactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef("MarianBombExplosion");
            bombImpactExplosion.timerAfterImpact = false;
            bombImpactExplosion.lifetimeAfterImpact = 0.01f;

            ProjectileController bombController = IceBomb.GetComponent<ProjectileController>();
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("MarianBombGhost") != null) bombController.ghostPrefab = CreateGhostPrefab("MarianBombGhost");
            bombController.startSound = "";
        }

        private static void createNapalm()
        {
            //Napalm = CloneProjectilePrefab("MolotovProjectileDotZone", "Napalm");
            NapalmLarge = CloneProjectilePrefab("MolotovProjectileDotZone", "Napalm");
        }
        private static void createStickyFire()
        {
            stickyFirePrefab = CloneProjectilePrefab("MolotovSingleProjectile", "StickyFireProjectile");
            //stickyFirePrefab = CloneProjectilePrefab("Fireball", "StickyFireProjectile");

            Log.Debug("Getting Impact explosion");
            ProjectileImpactExplosion bombImpactExplosion;
            if (bombImpactExplosion = stickyFirePrefab.GetComponent<ProjectileImpactExplosion>())
            {
                Log.Debug("HasBombImpacExplosion");
            }
            else
                bombImpactExplosion = stickyFirePrefab.AddComponent<ProjectileImpactExplosion>();

            bombImpactExplosion.lifetime = 2;

            bombImpactExplosion.destroyOnEnemy = true;
            bombImpactExplosion.destroyOnWorld = true;
            bombImpactExplosion.blastRadius = 10f;
            bombImpactExplosion.blastDamageCoefficient = 2;

            NapalmLarge.transform.localScale *= 10;
            bombImpactExplosion.childrenProjectilePrefab = NapalmLarge;
            bombImpactExplosion.childrenCount = 1;
            bombImpactExplosion.fireChildren = true;

            //stickyFirePrefab.GetComponent<Rigidbody>().drag = 1f;

            //bombImpactExplosion.childrenProjectilePrefab.transform.localScale *= 2;
            //bombImpactExplosion.blastRadius = 100f;

            //stickyFirePrefab.AddComponent<ConstantForce>();
            //stickyFirePrefab.GetComponent<ConstantForce>().force = new Vector3(0, -25, 0);
            bombImpactExplosion.impactEffect = Modules.Assets.marianEnergyProj;
            //bombImpactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef("StickyFireExplosion");

            Log.Debug("Getting Projectile Simple");
            ProjectileSimple BombProj;
            if (BombProj = stickyFirePrefab.GetComponent<ProjectileSimple>())
            {
                BombProj.desiredForwardSpeed = 200;
                BombProj.oscillateMagnitude = 0;
                BombProj.lifetime = 60;
            }
            else
                Log.Debug("Did not get ProjectileSimple");
            //BombProj.stopwatch = 60;

            stickyFirePrefab.GetComponent<AntiGravityForce>().antiGravityCoefficient = 1f;

            Log.Debug("Getting TourqueOnStart");
            ApplyTorqueOnStart torque;
            if (torque = stickyFirePrefab.GetComponent<ApplyTorqueOnStart>())
            {
                torque.localTorque = Vector3.zero;
                torque.randomize = false;
            }
            else
                Log.Debug("Did not get TorqueOnStart");



            ProjectileController bombController = stickyFirePrefab.GetComponent<ProjectileController>();
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("StickyFireProjectile") != null) bombController.ghostPrefab = CreateGhostPrefab("StickyFireProjectile");
            bombController.startSound = "";
        }

        private static void CreateBolt()
        {
            //boltPrefab = CloneProjectilePrefab("CrocoSpit", "MarianEnergyProjectile");
            boltPrefab = CloneProjectilePrefab("MageLightningboltBasic", "MarianEnergyProjectile");
            boltPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            ProjectileImpactExplosion bombImpactExplosion = boltPrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeProjImpactExplosion(bombImpactExplosion);
            ProjectileSimple pj = boltPrefab.GetComponent<ProjectileSimple>();
            pj.desiredForwardSpeed = 90;

            bombImpactExplosion.blastRadius = 1f;
            bombImpactExplosion.destroyOnEnemy = true;
            bombImpactExplosion.lifetime = 0.6f;
            bombImpactExplosion.impactEffect = Modules.Assets.marianEnergyProj;
            //bombImpactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef(Modules.Assets.boltExplosionEffect.GetComponent<EffectComponent>().soundName);

            //bombImpactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef("MurathMissileDetonation");

            bombImpactExplosion.destroyOnWorld = true;
            bombImpactExplosion.timerAfterImpact = false;
            bombImpactExplosion.lifetimeAfterImpact = 0.1f;
            ProjectileDamage pd = boltPrefab.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Generic;

            ProjectileController bombController = boltPrefab.GetComponent<ProjectileController>();
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("MarianEnergyProjectile") != null) bombController.ghostPrefab = CreateGhostPrefab("MarianEnergyProjectile");
            bombController.shouldPlaySounds = false;
            bombController.startSound = "";
            bombController.flightSoundLoop = null;
        }
        private static void InitializeProjImpactExplosion(ProjectileImpactExplosion projectileImpactExplosion)
        {
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.blastProcCoefficient = 1f;
            projectileImpactExplosion.blastRadius = 1f;
            projectileImpactExplosion.bonusBlastForce = Vector3.zero;
            projectileImpactExplosion.childrenCount = 0;
            projectileImpactExplosion.childrenDamageCoefficient = 0f;
            projectileImpactExplosion.childrenProjectilePrefab = null;
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.destroyOnWorld = false;
            projectileImpactExplosion.falloffModel = RoR2.BlastAttack.FalloffModel.None;
            //projectileImpactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef(EntityStates.Commando.CommandoWeapon.FirePistol2.firePistolSoundString);
            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.lifetime = 0f;
            projectileImpactExplosion.lifetimeAfterImpact = 0f;
            projectileImpactExplosion.lifetimeRandomOffset = 0f;
            projectileImpactExplosion.offsetForLifetimeExpiredSound = 0f;
            projectileImpactExplosion.timerAfterImpact = false;

            //projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.BlightOnHit;
        }

        private static void InitializeImpactExplosion(ProjectileImpactExplosion projectileImpactExplosion)
        {
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.blastProcCoefficient = 1f;
            projectileImpactExplosion.blastRadius = 1f;
            projectileImpactExplosion.bonusBlastForce = Vector3.zero;
            projectileImpactExplosion.childrenCount = 0;
            projectileImpactExplosion.childrenDamageCoefficient = 0f;
            projectileImpactExplosion.childrenProjectilePrefab = null;
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.destroyOnWorld = false;
            projectileImpactExplosion.falloffModel = RoR2.BlastAttack.FalloffModel.None;
            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.lifetime = 0f;
            projectileImpactExplosion.lifetimeAfterImpact = 0f;
            projectileImpactExplosion.lifetimeRandomOffset = 0f;
            projectileImpactExplosion.offsetForLifetimeExpiredSound = 0f;
            projectileImpactExplosion.timerAfterImpact = false;

            projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }
    }
}