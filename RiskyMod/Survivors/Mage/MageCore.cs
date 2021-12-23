﻿using RoR2;
using UnityEngine;
using System;
using EntityStates;

namespace RiskyMod.Survivors.Mage
{
    public class MageCore
    {
        public static bool enabled = true;
        public MageCore()
        {
            if (!enabled) return;
            ModifySkills(RoR2Content.Survivors.Mage.bodyPrefab.GetComponent<SkillLocator>());
        }
        private void ModifySkills(SkillLocator sk)
        {
            ModifyPrimaries(sk);
            ModifySpecials(sk);
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
            new M1Projectiles();
        }

        private void ModifySpecials(SkillLocator sk)
        {
            //SneedUtils.SneedUtils.DumpEntityStateConfig("EntityStates.Mage.Weapon.Flamethrower");
            for (int i = 0; i < sk.special.skillFamily.variants.Length; i++)
            {
                if (sk.special.skillFamily.variants[i].skillDef.activationState.stateType == typeof(EntityStates.Mage.Weapon.Flamethrower))
                {
                    EntityStates.RiskyMod.Mage.Flamethrower.flamethrowerEffectPrefab
                        = (GameObject)SneedUtils.SneedUtils.GetEntityStateFieldObject("EntityStates.Mage.Weapon.Flamethrower", "flamethrowerEffectPrefab");
                    sk.special.skillFamily.variants[i].skillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.RiskyMod.Mage.Flamethrower));
                    sk.special.skillFamily.variants[i].skillDef.skillDescriptionToken = "MAGE_SPECIAL_FIRE_DESCRIPTION_RISKYMOD";
                }
                else if (sk.special.skillFamily.variants[i].skillDef.activationState.stateType == typeof(EntityStates.Mage.FlyUpState))
                {
                    sk.special.skillFamily.variants[i].skillDef.skillDescriptionToken = "MAGE_SPECIAL_LIGHTNING_DESCRIPTION_RISKYMOD";

                    string keyword = Tweaks.Shock.enabled ? "KEYWORD_SHOCKING_RISKYMOD" : "KEYWORD_SHOCKING";

                    sk.special.skillFamily.variants[i].skillDef.keywordTokens = new string[] { keyword };
                    new IonSurgeTweaks();
                }
            }
        }
    }
}
