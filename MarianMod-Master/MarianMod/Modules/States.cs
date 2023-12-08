using MarianMod.SkillStates;
using MarianMod.SkillStates.BaseStates;
using System.Collections.Generic;
using System;

namespace MarianMod.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            Modules.Content.AddEntityState(typeof(BaseMeleeAttack));
            Modules.Content.AddEntityState(typeof(SlashCombo));

            Modules.Content.AddEntityState(typeof(Shoot));

            Modules.Content.AddEntityState(typeof(Roll));

            Modules.Content.AddEntityState(typeof(Marian_Grapple_simple));
            Modules.Content.AddEntityState(typeof(PrimaryFire));
            Modules.Content.AddEntityState(typeof(MarianIceBomb));
            Modules.Content.AddEntityState(typeof(GrappleSwitch));
            Modules.Content.AddEntityState(typeof(MarianFlare));
        }
    }
}