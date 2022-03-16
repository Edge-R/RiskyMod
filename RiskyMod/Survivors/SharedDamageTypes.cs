﻿using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors
{
    //Keep all the custom damagetypes in 1 place, in case there's a need to re-use them across different parts of the mod.
    public class SharedDamageTypes
    {
        public static DamageAPI.ModdedDamageType ProjectileRainForce;

        public static DamageAPI.ModdedDamageType AntiFlyingForce;
        public static DamageAPI.ModdedDamageType SawBarrier;

        public static DamageAPI.ModdedDamageType InterruptOnHit;

        public static DamageAPI.ModdedDamageType Blight7s;
        public static DamageAPI.ModdedDamageType Poison7s;

        public static DamageAPI.ModdedDamageType CrocoBiteHealOnKill;
        public static GameObject medkitEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MedkitHealEffect");

        public static DamageAPI.ModdedDamageType IgniteLevelScaled;

        public static DamageAPI.ModdedDamageType AlwaysIgnite;   //Used for Molten Perforatur due to not proccing


        public SharedDamageTypes()
        {
            InterruptOnHit = DamageAPI.ReserveDamageType();
            ProjectileRainForce = DamageAPI.ReserveDamageType();

            AntiFlyingForce = DamageAPI.ReserveDamageType();
            SawBarrier = DamageAPI.ReserveDamageType();

            Blight7s = DamageAPI.ReserveDamageType();
            Poison7s = DamageAPI.ReserveDamageType();
            CrocoBiteHealOnKill = DamageAPI.ReserveDamageType();

            IgniteLevelScaled = DamageAPI.ReserveDamageType();

            AlwaysIgnite = DamageAPI.ReserveDamageType();

            TakeDamage.ModifyInitialDamageActions += ApplyProjectileRainForce;
            TakeDamage.ModifyInitialDamageActions += ApplyAntiFlyingForce;

            OnHitEnemy.OnHitNoAttackerActions += ApplyInterruptOnHit;
            OnHitEnemy.OnHitNoAttackerActions += ApplyBlight7s;
            OnHitEnemy.OnHitNoAttackerActions += ApplyPoison7s;

            OnHitEnemy.OnHitAttackerActions += SawBarrierOnHit;

            OnHitEnemy.OnHitAttackerActions += ApplyIgniteLevelScaled;

            TakeDamage.OnDamageTakenAttackerActions += ApplyAlwaysIgnite;
        }

        private static void ApplyAlwaysIgnite(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(AlwaysIgnite))
            {
                float damageMultiplier = 0.5f;
                InflictDotInfo inflictDotInfo = new InflictDotInfo
                {
                    attackerObject = damageInfo.attacker,
                    victimObject = self.gameObject,
                    totalDamage = new float?(damageInfo.damage),
                    damageMultiplier = damageMultiplier,
                    dotIndex = DotController.DotIndex.Burn,
                    maxStacksFromAttacker = null
                };
                if (attackerBody.inventory)
                {
                    StrengthenBurnUtils.CheckDotForUpgrade(attackerBody.inventory, ref inflictDotInfo);
                }
                DotController.InflictDot(ref inflictDotInfo);
            }
        }

        private static void ApplyProjectileRainForce(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(SharedDamageTypes.ProjectileRainForce))
            {
                if (damageInfo.inflictor && damageInfo.inflictor.transform)
                {
                    Vector3 direction = -damageInfo.inflictor.transform.up;
                    CharacterBody cb = self.body;
                    if (cb)
                    {
                        //Scale force to match mass
                        Rigidbody rb = cb.rigidbody;
                        if (rb)
                        {
                            direction *= Mathf.Max(rb.mass / 100f, 1f);
                        }
                    }
                    damageInfo.force = 330f * direction;
                }
            }
        }

        //I dislike how this is reliant on specific enemy types and acts as an exception to the game mechanics.
        private static void ApplyIgniteLevelScaled(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(SharedDamageTypes.IgniteLevelScaled))
            {
                float burnDuration = 4f * damageInfo.procCoefficient; //4s is default ignite, 6s needed to always be able to kill Wisps with burn damage alone
                float damageMult = 1f;

                //!victimBody.isChampion //Only works on non-bosses
                if (victimBody.baseMaxHealth <= 100f)    //Only works on Wisps/Jellyfish/Beetles/Lemurians/Hermit Crabs
                {
                    burnDuration *= 1.5f;
                    //Downscale damage to attacker's base damage
                    //This may lose some additive damage bonuses but that shouldn't be too noticeable
                    damageMult = 1f / (1f + 0.2f * (attackerBody.level - 1f));

                    //Scale up damage based on enemy level
                    float targetLevel = Mathf.Max(victimBody.level, attackerBody.level);
                    damageMult *= 1f + 0.3f * (targetLevel - 1f);

                    //1.12 is breakpoint for 2shotting Beetles/Lemurians
                    damageMult = Mathf.Max(1.15f * damageMult, 1f);
                }

                DotController.InflictDot(victimBody.gameObject, damageInfo.attacker, DotController.DotIndex.Burn, burnDuration, damageMult);
            }
        }

        private static void ApplyInterruptOnHit(DamageInfo damageInfo, CharacterBody victimBody)
        {
            if (damageInfo.HasModdedDamageType(SharedDamageTypes.InterruptOnHit))
            {
                SetStateOnHurt component = victimBody.healthComponent.GetComponent<SetStateOnHurt>();
                if (component != null)
                {
                    component.SetStun(-1f);
                }
            }
        }

        private static void ApplyBlight7s(DamageInfo damageInfo, CharacterBody victimBody)
        {
            if (damageInfo.HasModdedDamageType(SharedDamageTypes.Blight7s))
            {
                DotController.InflictDot(victimBody.gameObject, damageInfo.attacker, DotController.DotIndex.Blight, 7f, 1f);
            }
        }
        private static void ApplyPoison7s(DamageInfo damageInfo, CharacterBody victimBody)
        {
            if (damageInfo.HasModdedDamageType(SharedDamageTypes.Poison7s))
            {
                DotController.InflictDot(victimBody.gameObject, damageInfo.attacker, DotController.DotIndex.Poison, 7f, 1f);
            }
        }

        private static void ApplyAntiFlyingForce(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(AntiFlyingForce))
            {
                Vector3 direction = Vector3.down;
                CharacterBody cb = self.body;
                if (cb && cb.isFlying)
                {
                    //Scale force to match mass
                    Rigidbody rb = cb.rigidbody;
                    if (rb)
                    {
                        if (damageInfo.force.y > 0f)
                        {
                            damageInfo.force.y = 0f;
                        }

                        direction *= Mathf.Min(10f, Mathf.Max(rb.mass / 100f, 1f));
                        damageInfo.force += 1600f * direction;
                    }
                }
            }
        }

        private void SawBarrierOnHit(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(SawBarrier))
            {
                if (attackerBody.healthComponent)
                {
                    attackerBody.healthComponent.AddBarrier(attackerBody.healthComponent.fullCombinedHealth * 0.006f);
                }
            }
        }
    }
}
