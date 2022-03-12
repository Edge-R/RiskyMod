﻿using UnityEngine;
using RoR2;
using RiskyMod.Fixes;

namespace RiskyMod.Tweaks
{
    public class RunScaling
    {
		public static bool enabled = true;
		public static float rewardMultiplier = 0.85f;

		public static GameModeIndex classicRunIndex;
		public static GameModeIndex simulacrumIndex;

        public RunScaling()
        {
			if (!enabled) return;


			On.RoR2.GameModeCatalog.LoadGameModes += (orig) =>
			{
				orig();
				simulacrumIndex = GameModeCatalog.FindGameModeIndex("InfiniteTowerRun");
				classicRunIndex = GameModeCatalog.FindGameModeIndex("ClassicRun");
			};

			On.RoR2.Run.RecalculateDifficultyCoefficentInternal += (orig, self) =>
            {
				int playerCount = self.participatingPlayerCount;
				float time = self.GetRunStopwatch() * 0.0166666675f; //Convert stopwatch(seconds) into minutes. Why is this Floored in vanilla, and why does it still move anyways despite that?

				DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(self.selectedDifficulty);
                float playerFactor = 0.7f + playerCount * 0.3f;
				float timeFactor = time * 0.1111111111f * difficultyDef.scalingValue;//* Mathf.Pow(playerCount, 0.15f)
				float stageFactor = Mathf.Pow(1.18f, self.stageClearCount / 5);  //Exponential scaling happens on a per-loop basis
				float finalDifficulty = (playerFactor + timeFactor) * stageFactor;
				self.compensatedDifficultyCoefficient = finalDifficulty;
				self.difficultyCoefficient = finalDifficulty;

				//Untitled Difficulty Mod overwrites Run.ambientLevelCap
				self.ambientLevel = Mathf.Min(3f * (finalDifficulty - playerFactor) + 1f, RemoveLevelCap.enabled ? RemoveLevelCap.maxLevel : Run.ambientLevelCap);

				//Vanilla code
				int ambientLevelFloor = self.ambientLevelFloor;
				self.ambientLevelFloor = Mathf.FloorToInt(self.ambientLevel);
				if (ambientLevelFloor != self.ambientLevelFloor && ambientLevelFloor != 0 && self.ambientLevelFloor > ambientLevelFloor)
				{
					self.OnAmbientLevelUp();
				}
			};

			On.RoR2.CombatDirector.DirectorMoneyWave.Update += (orig, self, deltaTime, difficultyCoefficient) =>
			{
				if (Run.instance.gameModeIndex == classicRunIndex)
				{
					float stageFactor = Run.instance.stageClearCount < 4 ? Mathf.Pow(1.1f, Run.instance.stageClearCount) : 1.5f;//Needs cap to prevent game from turning into a slideshow. Uncapping it causes excessive T2 Elite spam.
					difficultyCoefficient *= stageFactor;
				}
				return orig(self, deltaTime, difficultyCoefficient);
			};

			On.RoR2.CombatDirector.Awake += (orig, self) =>
			{
				if (Run.instance.gameModeIndex == classicRunIndex)
				{
					self.creditMultiplier *= 1.1f;
				}
				orig(self);
			};

			On.RoR2.DeathRewards.OnKilledServer += (orig, self, damageReport) =>
			{
				if (Run.instance.gameModeIndex == classicRunIndex)
				{
					self.goldReward = (uint)Mathf.CeilToInt(Mathf.Pow(self.goldReward, 0.9f) / Mathf.Pow(1.2f, Run.instance.stageClearCount / 5));
				}
				orig(self, damageReport);
			};
		}

		private float GetScaledReward()
		{
			return 1.1f * Run.instance.stageClearCount < 4 ? Mathf.Pow(1.1f, Run.instance.stageClearCount) : 1.5f;
		}
	}
}
