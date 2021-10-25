﻿using RiskyMod.Items.Boss;
using RiskyMod.Items.Common;
using RiskyMod.Items.Legendary;
using RiskyMod.Items.Lunar;
using RiskyMod.Items.Uncommon;
using RoR2;
using System.Runtime.CompilerServices;

namespace RiskyMod.Items
{
    public class ItemsCore
    {
        public static bool itemTweaksEnabled = true;
        public static bool uncommonEnabled = true;
        public static bool commonEnabled = true;
        public static bool legendaryEnabled = true;
        public static bool bossEnabled = true;
        public static bool lunarEnabled = true;

        public ItemsCore()
        {
            if (!itemTweaksEnabled) return;
            Planula.enabled = Stealthkit.enabled || Razorwire.enabled || SquidPolyp.enabled;
            ModifyCommon();
            ModifyUncommon();
            ModifyLegendary();
            ModifyBoss();
            ModifyLunar();
        }

        private void ModifyCommon()
        {
            if (!commonEnabled) return;
            new BisonSteak();
            new MonsterTooth();
            new CritGlasses();
            new Fireworks();
            new StickyBomb();
            new Crowbar();
            new Warbanner();
            new Gasoline();
            new RepArmor();
        }

        private void ModifyUncommon()
        {
            if (!uncommonEnabled) return;
            new Predatory();
            new Chronobauble();
            new LeechingSeed();
            new AtG();
            new ElementalBands();
            new Bandolier();
            new Stealthkit();
            new WillOWisp();
            new SquidPolyp();
            new Ukulele();
            new Razorwire();
            new RoseBuckler();
            new Guillotine();
            new Berzerker();
        }

        private void ModifyLegendary()
        {
            if (!legendaryEnabled) return;
            new Tesla();
            new FrostRelic();
            new CeremonialDagger();
            new MeatHook();
            new LaserTurbine();
            new Headhunter();
            new Headstompers();
            new NovaOnHeal();
        }

        private void ModifyBoss()
        {
            if (!bossEnabled) return;
            new QueensGland();
            new MoltenPerf();
            new ChargedPerf();
            new Shatterspleen();
            new Knurl();
            new Disciple();
            new Planula();
            new GenesisLoop();
        }

        private void ModifyLunar()
        {
            if (!lunarEnabled) return;
            new ShapedGlass();
        }

        public static void AddToAIBlacklist(string itemName)
        {
            ItemIndex i = ItemCatalog.FindItemIndex(itemName);
            if (i != ItemIndex.None)
            {
                AddToAIBlacklist(i);
            }
        }

        public static void AddToAIBlacklist(ItemIndex index)
        {
            ItemDef itemDef = ItemCatalog.GetItemDef(index);
            if (itemDef.DoesNotContainTag(ItemTag.AIBlacklist))
            {
                System.Array.Resize(ref itemDef.tags, itemDef.tags.Length + 1);
                itemDef.tags[itemDef.tags.Length - 1] = ItemTag.AIBlacklist;
            }
        }

        public static string ToPercent(float coefficient)
        {
            return coefficient.ToString("P0").Replace(" ", "").Replace(",", "");
        }
    }
}