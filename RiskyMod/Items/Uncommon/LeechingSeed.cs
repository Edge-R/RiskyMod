﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using System;

namespace RiskyMod.Items.Uncommon
{
    public class LeechingSeed
    {
        public static bool enabled = true;
        public LeechingSeed()
        {
			if (!enabled) return;
			On.RoR2.ItemCatalog.Init += (orig) =>
			{
				orig();
				HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Seed);
			};

			//Remove vanilla effect.
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);
				c.GotoNext(
					 x => x.MatchLdsfld(typeof(RoR2Content.Items), "Seed")
					);
				c.Remove();
				c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
			};

			TakeDamage.OnHpLostAttackerActions += HealOnHit;
		}

		private void HealOnHit(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody, Inventory inventory, float hpLost)
		{
			if (damageInfo.procCoefficient > 0f && !damageInfo.procChainMask.HasProc(ProcType.HealOnHit) && attackerBody.inventory && attackerBody.healthComponent)
            {
				int seedCount = attackerBody.inventory.GetItemCount(RoR2Content.Items.Seed);
				if (seedCount > 0)
                {
					float toHeal = hpLost * (0.025f + 0.025f * seedCount);
					damageInfo.procChainMask.AddProc(ProcType.HealOnHit);
					attackerBody.healthComponent.Heal(toHeal * damageInfo.procCoefficient, damageInfo.procChainMask);
				}
            }
		}
	}
}
