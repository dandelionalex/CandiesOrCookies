using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PickMaster.Model
{
    public class SettingConfig
    {
        public List<RollerModel> Rollers = new List<RollerModel>();
        public int Id = 0;
        public int BaseDuration = 80;
        public int BaseGoldenRainDuration = 12;
        public float BaseRollChance = 0.32f;
        public float BaseRollerCoefficient = 1; // increase or decrease golden bar progress
        public int BaseUpgradePrice = 20000;
        public float BaseSpawnDelay = 0.5f;
        public float BaseSpawnDelayDecrease = 0.1f;
        public float BaseSpawnDelayDecreaseTimeout = 10;
        public float BaseSpawnDelayMinimum = 0.3f;
        public int InitialRollerCount = 4;
        public float BaseGoldBonusModifier = 1;
        public string SettingName = string.Empty;
        /// <summary>
        /// upgrade to next level price
        /// </summary>
        public int UnlockLevelPrice = 100;
        
        public List<LevelConfig> Levels = new List<LevelConfig>();
        
        public int GetLevelDuration(int level)
        {
            if (level >= Levels.Count)
                return BaseDuration;

            return Levels[level].Duration;
        }

        public int GetLevelLength()
        {
            return Rollers.Count;
        }

        public int GetGoldenRainDuration(int level)
        {
            if (level >= Levels.Count)
                return BaseGoldenRainDuration;
            
            return Levels[level].GoldenRainDuration;
        }

        public RollerModel GetRoller(string rollerId)
        {
            return Rollers.FirstOrDefault(r => r.RollerId == rollerId);
        }

        /// <summary>
        /// increase or decrease golden bar progress
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public float GetRollerCoefficient(int level)
        {
            if (level >= Levels.Count)
                return BaseRollerCoefficient;
            
            return Levels[level].RollerCoefficient;
        }

        public int GetUpgradePrice(int level)
        {
            if (level >= Levels.Count)
                return BaseUpgradePrice;
            
            return Levels[level].NextUpgradePrice;
        }

        /// <summary>
        /// Chance of positive Roller
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public float GetRollChance(int level)
        {
            if (level >= Levels.Count)
                return BaseRollChance;
            
            return Levels[level].RollChance;
        }
        
        public float GetSpawnDelay(int level)
        {
            if (level >= Levels.Count)
                return BaseSpawnDelay;
            
            return Levels[level].SpawnDelay;
        }
        
        public float GetSpawnDelayDecrease(int level)
        {
            if (level >= Levels.Count)
                return BaseSpawnDelayDecrease;
            
            return Levels[level].SpawnDelayDecrease;
        }
        
        public float GetSpawnDelayDecreaseTimeout(int level)
        {
            if (level >= Levels.Count)
                return BaseSpawnDelayDecreaseTimeout;
            
            return Levels[level].SpawnDelayDecreaseTimeout;
        }
        
        public float GetSpawnDelayDecreaseMinimum(int level)
        {
            if (level >= Levels.Count)
                return BaseSpawnDelayMinimum;
            
            return Levels[level].SpawnDelayMinimum;
        }
        
        public float GetGoldBonusModifier(int level)
        {
            if (level >= Levels.Count)
                return BaseGoldBonusModifier;
            
            return Levels[level].GoldBonusModifier;
        }
    }
}