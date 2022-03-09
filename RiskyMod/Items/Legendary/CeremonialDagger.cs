﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using RoR2.Projectile;
using R2API;
using System;
using RoR2;

namespace RiskyMod.Items.Legendary
{
    class CeremonialDagger
    {
        public static bool enabled = true;
        public static GameObject daggerPrefab;
        public CeremonialDagger()
        {
            if (!enabled || !RiskyMod.disableProcChains) return;

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(GlobalEventManager.CommonAssets), "daggerPrefab")
                    );
                c.Index++;
                c.EmitDelegate<Func<GameObject, GameObject>>((oldPrefab) =>
                {
                    return CeremonialDagger.daggerPrefab;
                });
            };

            daggerPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/DaggerProjectile").InstantiateClone("RiskyMod_CeremonialDaggerProjectile", true);
            ProjectileController pc = daggerPrefab.GetComponent<ProjectileController>();
            pc.procCoefficient = 0f;
            R2API.ContentAddition.AddProjectile(daggerPrefab);
        }
    }
}
