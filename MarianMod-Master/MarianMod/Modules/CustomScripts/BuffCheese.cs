using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.Projectile;
using UnityEngine.Networking;
using UnityEngine;

namespace MarianMod.Modules.CustomScripts
{
    class BuffCheese : MonoBehaviour
    {
        public bool clear = false;
        ProjectileController pc;
        public void Start()
        {
            if (NetworkServer.active)
            {
                //ProjectileController.Networkowner
                pc = this.GetComponent<ProjectileController>();
                ProjectileDamage pd = this.GetComponent<ProjectileDamage>();
                //pc.Networkowner.GetComponent<CharacterBody>().AddBuff(RoR2Content.Buffs.Cloak);
                pc.Networkowner.GetComponent<CharacterBody>().ClearTimedBuffs(Modules.Buffs.MissileBuff);
                for (int i = 0; i < pd.damage; i++)
                    pc.Networkowner.GetComponent<CharacterBody>().AddTimedBuff(Modules.Buffs.MissileBuff, 1);

            }
            Destroy(this.gameObject);
        }
    }
}
