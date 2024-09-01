using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;

namespace MarianMod.Modules.CustomScripts
{
    class SpriteManager : MonoBehaviour
    {
        public Transform[] indicators = new Transform[50];
        public Transform[] Targets = new Transform[50];
        GameObject Camera;
        float Range = MarianMod.SkillStates.MarianFlare.Range;

        Indicator[] MissileInidicator = new Indicator[50];
        int ScanMax = 50;


        private void OnEnable()
        {
            for (int i = 0; i < ScanMax; i++)
            {
                this.MissileInidicator[i].active = true;
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < ScanMax; i++)
            {
                this.MissileInidicator[i].active = false;
            }
        }

        public void Start()
        {
            Camera = GameObject.Find("Main Camera(Clone)");
            for (int i = 0; i < ScanMax; i++)
            {
                this.MissileInidicator[i] = new Indicator(this.gameObject, Modules.Assets.MissileSprite);
            }
        }
        public void LateUpdate()
        {
            UpdateIndicators();
        }

        public void UpdateIndicators()
        {
            //int count = 0;
            for (int i = 0; i < Targets.Length; i++)
            {
                this.MissileInidicator[i].targetTransform = (Targets[i] ? Targets[i].transform : null);
            }
            for (int i = Targets.Length; i < ScanMax; i++)
            {
                this.MissileInidicator[i].targetTransform = null;
            }
        }
    }
}
