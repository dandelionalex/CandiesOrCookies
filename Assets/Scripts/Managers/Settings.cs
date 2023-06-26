using System.Collections.Generic;
using PickMaster.Model;
using UnityEngine;

namespace PickMaster.Managers
{
    public class Settings
    {
        public SettingConfig GetSettingConfig(int settingId)
        {
            if(settingId >= settings.Count)
                return null;
            
            return settings[settingId];
        }

        private List<SettingConfig> settings = new List<SettingConfig>()
        {
            new SettingConfig()
            {
                UnlockLevelPrice = 8000,
                Id = 0,
                SettingName = "Candy Land",
                Rollers = new List<RollerModel>()
                {
                    new RollerModel("Gummy_C_01", "Gummy_C_01", "Gummy", 1f),
                    new RollerModel("Mintstar_05", "Mintstar_05", "Ministar", 1f),
                    new RollerModel("Truffle", "Truffle", "Truffle", 1f),
                    new RollerModel("CookieChoc_03", "CookieChoc_03", "Cookie", 1f),
                    new RollerModel("Candy_C", "Candy_C", "Candy_1 1", 1f),
                    new RollerModel("Lollipop_A_03", "Lollipop_A_03", "Lollipop", 1f),
                    new RollerModel("Macaroon_01", "Macaroon_01", "Maccaroon", 1f),
                    new RollerModel("Donut_A_05", "Donut_A_05", "Donut", 1f)
                },
                BaseRollChance = 0.32f,
                Levels = new List<LevelConfig>()
                {
                    new LevelConfig()
                    {
                        Id = 0,
                        Duration = 20,
                        GoldenRainDuration = 4,
                        RollChance = 0.9f,
                        NextUpgradePrice = 500,
                        RollerCoefficient = 0.1f,
                        GoldBonusModifier = 1
                    },
                    new LevelConfig()
                    {
                        Id = 1,
                        Duration = 20,
                        GoldenRainDuration = 4,
                        RollChance = 0.9f,
                        NextUpgradePrice = 2000,
                        RollerCoefficient = 0.084f,
                        GoldBonusModifier = 1.1f
                    },
                    new LevelConfig()
                    {
                        Id = 2,
                        Duration = 20,
                        GoldenRainDuration = 5,
                        RollChance = 0.9f,
                        NextUpgradePrice = 4000,
                        RollerCoefficient = 0.077f,
                        GoldBonusModifier = 1.2f
                    },
                    new LevelConfig()
                    {
                        Id = 3,
                        Duration = 20,
                        GoldenRainDuration = 5,
                        RollChance = 0.9f,
                        NextUpgradePrice = 6000,
                        RollerCoefficient = 0.072f,
                        GoldBonusModifier = 1.3f
                    },
                    new LevelConfig()
                    {
                        Id = 4,
                        Duration = 20,
                        GoldenRainDuration = 5,
                        RollChance = 0.9f,
                        NextUpgradePrice = 7500,
                        RollerCoefficient = 0.067f,
                        GoldBonusModifier = 1.4f
                    }
                }
            },
            new SettingConfig()
            {
                UnlockLevelPrice = 50000,
                Id = 1,
                SettingName = "Aztec Treazures",
                Rollers = new List<RollerModel>()
                {
                    new RollerModel("Block_Black", "Block_Black", "Pyramid_black", 1f),
                    new RollerModel("Block_Violet", "Block_Violet", "Pyramid_Purple", 1f),
                    new RollerModel("Diamond_White", "Diamond_White", "Diamond_White", 1f),
                    new RollerModel("Diamond_Yellow", "Diamond_Yellow", "Diamond_Yellow", 1f),
                    new RollerModel("Flat_Blue", "Flat_Blue", "Round_Blue", 1f),
                    new RollerModel("Flat_Green", "Flat_Green", "Round_Green", 1f),
                    new RollerModel("Heart_Black", "Heart_Black", "Heart_Black", 1f),
                    new RollerModel("Heart_Red", "Heart_Red", "Heart_red", 1f),
                    new RollerModel("Ingot_Blue", "Ingot_Blue", "Long_Blue", 1f),
                    new RollerModel("Ingot_White", "Ingot_White", "Long_white", 1f),
                    new RollerModel("Star_Violet", "Star_Violet", "Star_Purple", 1f),
                    new RollerModel("Star_yellow", "Star_yellow", "Star_Yellow", 1f),
                    new RollerModel("Stick_Green", "Stick_Green", "Pile_Green", 1f),
                    new RollerModel("Stick_Red", "Stick_Red", "Pile_Red", 1f),
                },
                BaseRollChance = 0.32f,
                Levels = new List<LevelConfig>()
                {
                    new LevelConfig()
                    {
                        Id = 0,
                        Duration = 20,
                        GoldenRainDuration = 5,
                        RollChance = 0.9f,
                        NextUpgradePrice = 9000,
                        RollerCoefficient = 0.067f,
                        GoldBonusModifier = 1.5f
                    },
                    new LevelConfig()
                    {
                        Id = 1,
                        Duration = 20,
                        GoldenRainDuration = 5,
                        RollChance = 0.9f,
                        NextUpgradePrice = 9500,
                        RollerCoefficient = 0.067f,
                        GoldBonusModifier = 1.6f
                    },
                    new LevelConfig()
                    {
                        Id = 2,
                        Duration = 20,
                        GoldenRainDuration = 5,
                        RollChance = 0.9f,
                        NextUpgradePrice = 10000,
                        RollerCoefficient = 0.067f,
                        GoldBonusModifier = 1.7f
                    },
                    new LevelConfig()
                    {
                        Id = 3,
                        Duration = 20,
                        GoldenRainDuration = 5,
                        RollChance = 0.9f,
                        NextUpgradePrice = 10500,
                        RollerCoefficient = 0.067f,
                        GoldBonusModifier = 1.8f
                    },
                    new LevelConfig()
                    {
                        Id = 4,
                        Duration = 20,
                        GoldenRainDuration = 5,
                        RollChance = 0.9f,
                        NextUpgradePrice = 11500,
                        RollerCoefficient = 0.067f,
                        GoldBonusModifier = 1.9f
                    },
                    new LevelConfig()
                    {
                        Id = 4,
                        Duration = 20,
                        GoldenRainDuration = 5,
                        RollChance = 0.9f,
                        NextUpgradePrice = 12000,
                        RollerCoefficient = 0.067f,
                        GoldBonusModifier = 2.0f
                    },
                    new LevelConfig()
                    {
                        Id = 5,
                        Duration = 20,
                        GoldenRainDuration = 5,
                        RollChance = 0.9f,
                        NextUpgradePrice = 12500,
                        RollerCoefficient = 0.067f,
                        GoldBonusModifier = 2.1f
                    },
                    new LevelConfig()
                    {
                        Id = 6,
                        Duration = 20,
                        GoldenRainDuration = 5,
                        RollChance = 0.9f,
                        NextUpgradePrice = 13000,
                        RollerCoefficient = 0.067f,
                        GoldBonusModifier = 2.2f
                    },
                    new LevelConfig()
                    {
                        Id = 7,
                        Duration = 20,
                        GoldenRainDuration = 5,
                        RollChance = 0.9f,
                        NextUpgradePrice = 13500,
                        RollerCoefficient = 0.067f,
                        GoldBonusModifier = 2.3f
                    },
                    new LevelConfig()
                    {
                        Id = 8,
                        Duration = 20,
                        GoldenRainDuration = 5,
                        RollChance = 0.9f,
                        NextUpgradePrice = 14500,
                        RollerCoefficient = 0.067f,
                        GoldBonusModifier = 2.4f
                    },
                    new LevelConfig()
                    {
                        Id = 9,
                        Duration = 20,
                        GoldenRainDuration = 5,
                        RollChance = 0.9f,
                        NextUpgradePrice = 15000,
                        RollerCoefficient = 0.067f,
                        GoldBonusModifier = 2.5f
                    }
                }
            }
        };
    }
}
