﻿using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using RoR2.Orbs;
using System.Collections.Generic;
using UnityEngine;

namespace RiskyMod
{
    //Keep all the custom damagetypes in 1 place, in case there's a need to re-use them across different parts of the mod.
    public class SharedDamageTypes
    {
        public static DamageAPI.ModdedDamageType ProjectileRainForce;

        public static DamageAPI.ModdedDamageType AntiFlyingForce;
        public static DamageAPI.ModdedDamageType SawBarrier;

        public static DamageAPI.ModdedDamageType InterruptOnHit;

        public static DamageAPI.ModdedDamageType Slow50For5s;

        public static DamageAPI.ModdedDamageType Blight7s;
        public static DamageAPI.ModdedDamageType Poison7s;

        public static DamageAPI.ModdedDamageType CrocoBiteHealOnKill;
        public static GameObject medkitEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MedkitHealEffect");

        public static DamageAPI.ModdedDamageType AlwaysIgnite;   //Used for Molten Perforator due to not proccing


        public SharedDamageTypes()
        {
            InterruptOnHit = DamageAPI.ReserveDamageType();
            ProjectileRainForce = DamageAPI.ReserveDamageType();

            AntiFlyingForce = DamageAPI.ReserveDamageType();
            SawBarrier = DamageAPI.ReserveDamageType();

            Blight7s = DamageAPI.ReserveDamageType();
            Poison7s = DamageAPI.ReserveDamageType();
            CrocoBiteHealOnKill = DamageAPI.ReserveDamageType();

            AlwaysIgnite = DamageAPI.ReserveDamageType();

            Slow50For5s = DamageAPI.ReserveDamageType();

            TakeDamage.ModifyInitialDamageActions += ApplyProjectileRainForce;
            TakeDamage.ModifyInitialDamageActions += ApplyAntiFlyingForce;

            OnHitEnemy.OnHitNoAttackerActions += ApplyInterruptOnHit;
            OnHitEnemy.OnHitNoAttackerActions += ApplyBlight7s;
            OnHitEnemy.OnHitNoAttackerActions += ApplyPoison7s;
            OnHitEnemy.OnHitNoAttackerActions += ApplySlow50For5s;

            OnHitEnemy.OnHitAttackerActions += ApplySawBarrierOnHit;

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
                    totalDamage = new float?(damageInfo.damage * damageMultiplier),
                    damageMultiplier = 1f,
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

        private static void ApplySawBarrierOnHit(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(SawBarrier))
            {
                if (attackerBody.healthComponent)
                {
                    attackerBody.healthComponent.AddBarrier(attackerBody.healthComponent.fullCombinedHealth * 0.006f);
                }
            }
        }

        private static void ApplySlow50For5s(DamageInfo damageInfo, CharacterBody victimBody)
        {
            if (damageInfo.HasModdedDamageType(Slow50For5s))
            {
                victimBody.AddTimedBuff(RoR2Content.Buffs.Slow50, 5f);
            }
        }
    }
}
