using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;

namespace MarianMod.Modules.CustomScripts
{
    class PlaySoundOnDeath : MonoBehaviour
    {
        public void Start()
        {          
            Util.PlaySound(EntityStates.Commando.CommandoWeapon.FireBarrage.fireBarrageSoundString, this.gameObject);
        }
    }
}
