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
    class PoisonBomb : MarianIceBomb
    {
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return base.GetMinimumInterruptPriority();
        }

        public override GameObject setPrefab()
        {
            return Modules.Projectiles.PoisonBomb;
        }

        public override float setMult()
        {
            return 50;
        }
    }
}
