using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;

namespace MarianMod.Modules.CustomScripts
{
    class PlaySoundOnDeath : MonoBehaviour
    {
        public string sound = EntityStates.Commando.CommandoWeapon.FireBarrage.fireBarrageSoundString;
        public bool ice = true;
        public void Start()
        {
            if (ice)
                Util.PlaySound(EntityStates.Commando.CommandoWeapon.FireBarrage.fireBarrageSoundString, this.gameObject);
            else
            {
                for(int i = 0; i < 3; i++)
                    Util.PlaySound(EntityStates.Engi.EngiMissilePainter.Paint.enterSoundString, this.gameObject);
            }
        }
    }
}
