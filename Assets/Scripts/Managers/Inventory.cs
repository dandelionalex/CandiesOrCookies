using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Crashlytics;
using Newtonsoft.Json;
using PickMaster.DI.Signals;
using PickMaster.Enums;
using PickMaster.Model;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace PickMaster.Managers
{
    public class Inventory : IInitializable
    {
        public Action<int> BalanceUpdated;
        public int Balance
        {
            get
            {
                return PlayerPrefs.GetInt(PlayerPrefsKeys.BALANCE, 0);
            }
            set
            {
                BalanceUpdated?.Invoke(value);
                PlayerPrefs.SetInt(PlayerPrefsKeys.BALANCE, value);
            }
        }

        private int round = -1;
        public int Round
        {
            get
            {
                if(round == -1)
                    round = PlayerPrefs.GetInt(PlayerPrefsKeys.ROUND, 0);
                
                return round;
            }
            set
            {
                round = value;
                PlayerPrefs.SetInt(PlayerPrefsKeys.ROUND, round);
            }
        }
        
        private int tutorialStep = -1;
        public int TutorialStep
        {
            get
            {
                if(tutorialStep == -1)
                    tutorialStep = PlayerPrefs.GetInt(PlayerPrefsKeys.TUTORIAL_STEP, 0);
                
                return tutorialStep;
            }
            set
            {
                tutorialStep = value;
                if(tutorialStep > 2)
                    PlayerPrefs.SetInt(PlayerPrefsKeys.TUTORIAL_STEP, tutorialStep);
            }
        }

        private int currentSetting = -1;
        public int CurrentSetting {             
            get
            {
                if(currentSetting == -1)
                    currentSetting = PlayerPrefs.GetInt(PlayerPrefsKeys.CURRENT_SETTING, 0);
                
                return currentSetting;
            }
            set
            {
                currentSetting = value;
                PlayerPrefs.SetInt(PlayerPrefsKeys.CURRENT_SETTING, currentSetting);
            } }

        private List<SettingModel> settingsList;
        private readonly Settings settings;
        private readonly SignalBus signalBus;
            
        public Inventory(Settings settings, SignalBus signalBus)
        {
            this.settings = settings;
            this.signalBus = signalBus;
        }
        
        public SettingModel GetSetting(int settingId)
        {
            if(settingsList == null)
                Load();
            
            return settingsList.FirstOrDefault(l => l.Id == settingId);
        }

        public SettingModel GetCurrentSetting()
        {
            return GetSetting(CurrentSetting);
        }

        public int GetCurrentSettingLevel()
        {
            return GetCurrentSetting().OpenItems.Count - settings.GetSettingConfig(CurrentSetting).InitialRollerCount;
        }
        
        private void Load()
        {
            var levelsString = PlayerPrefs.GetString(PlayerPrefsKeys.LEVELS, String.Empty);
            Debug.Log($"LOAD LEVEL levelsString: {levelsString}");
            if (levelsString == String.Empty)
            {
                settingsList = new List<SettingModel>()
                {
                    GenerateSettingModel(0)
                };
                Save();
            }
            else
            {
                try
                {
                    settingsList = JsonConvert.DeserializeObject<List<SettingModel>>(levelsString);
                }
                catch (Exception ex)
                {
                    //crachlitics send messge
                    settingsList = new List<SettingModel>()
                    {
                        GenerateSettingModel(0)
                    };
                    Save();
                    Crashlytics.Log($"Error while parse settingsList config {ex.ToString()}");
                }
            }
        }

        private SettingModel GenerateSettingModel(int id)
        {
            var lvl = new SettingModel {Id = id};
            var setting = settings.GetSettingConfig(id);
            var shuffled = setting.Rollers.OrderBy(a => Guid.NewGuid()).ToList();
            for (int i = 0; i < 4; i++)
            {
                lvl.OpenItems.Add(shuffled[i].RollerId);
            }

            return lvl;
        }

        public RollerModel LevelUpgrade()
        {
            var rollers = settings.GetSettingConfig(CurrentSetting).Rollers;
            var currentLevel = GetSetting(CurrentSetting);
            if (rollers.Count == currentLevel.OpenItems.Count)
                return null;
            
            var availableList = rollers.Where(r => !currentLevel.OpenItems.Contains(r.RollerId)).ToList();
            var buyItem = availableList[Random.Range(0, availableList.Count)];
            currentLevel.OpenItems.Add(buyItem.RollerId);
            signalBus.TryFire(new LevelUpgradedSignal(buyItem));
            Save();
            return buyItem;
        }
        
        public void Save()
        {
            var levelString = JsonConvert.SerializeObject(settingsList);
            Debug.Log($"SAVE LEVEL levelString {levelString}");
            PlayerPrefs.SetString(PlayerPrefsKeys.LEVELS, levelString);
            PlayerPrefs.SetInt(PlayerPrefsKeys.CURRENT_LEVEL, CurrentSetting);
        }

        public void OpenSetting(int id)
        {
            settingsList.Add(GenerateSettingModel(id));
            Save();
        }

        public SettingUnlockState SettingUnlockAvailable()
        {
            Debug.Log($"Rollers.Count {settings.GetSettingConfig(CurrentSetting).Rollers.Count }, OpenItems: {GetCurrentSetting().OpenItems.Count}");
            if (settings.GetSettingConfig(CurrentSetting).Rollers.Count == GetCurrentSetting().OpenItems.Count)
            {
                if (settings.GetSettingConfig(CurrentSetting + 1) == null)
                    return SettingUnlockState.ComingSoon;

                if (GetSetting(CurrentSetting + 1) != null)
                    return SettingUnlockState.UpgradeDone;
                
                if(Balance>= settings.GetSettingConfig(CurrentSetting).UnlockLevelPrice)
                    return SettingUnlockState.UpgradeAvailable;
                
                return SettingUnlockState.UpgradeAvailableButNotAffordable;
            }
            
            return SettingUnlockState.UpgradeNotAvailable;
        }
        
        public void Initialize()
        {
            if(settingsList == null)
                Load();
        }
    }
}