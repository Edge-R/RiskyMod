﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Items.DLC1.Void
{
    public class SaferSpaces
    {
        public static bool enabled = true;
        public SaferSpaces()
        {
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                bool error = true;
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                    x => x.MatchLdfld<HealthComponent>("body"),
                     x => x.MatchLdsfld(typeof(DLC1Content.Buffs), "BearVoidReady"),
                     x => x.MatchCallvirt<CharacterBody>("RemoveBuff")
                    ))
                {
                    c.Index++;
                    c.EmitDelegate<Func<CharacterBody, CharacterBody>>(body =>
                    {
                        body.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.1f);
                        return body;
                    });

                    if (c.TryGotoNext(
                        x => x.MatchLdcR4(15f)
                        ))
                    {
                        c.Next.Operand = 20f;
                        error = false;
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: SaferSpaces IL Hook failed");
                }
            };
        }
    }
}
