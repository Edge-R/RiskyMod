﻿using RiskyMod.Survivors.Bandit2;
using RiskyMod.Survivors.Captain;
using RiskyMod.Survivors.Commando;
using RiskyMod.Survivors.Croco;
using RiskyMod.Survivors.Engi;
using RiskyMod.Survivors.Huntress;
using RiskyMod.Survivors.Loader;
using RiskyMod.Survivors.Mage;
using RiskyMod.Survivors.Toolbot;
using RiskyMod.Survivors.Treebot;

namespace RiskyMod.Survivors
{
    public class SurvivorsCore
    {
        public static bool enabled = true;
        public SurvivorsCore()
        {
            if (!enabled) return;

            new SharedDamageTypes();

            new Bandit2Core();
            new CaptainCore();
            new CommandoCore();
            new EngiCore();
            new HuntressCore();
            new ToolbotCore();
            new TreebotCore();
            new CrocoCore();
            new LoaderCore();
            new MageCore();
        }
    }
}
