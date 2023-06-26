using System.Collections.Generic;
using DG.Tweening;
using Enums;
using PickMaster.DI.Signals;
using PickMaster.Enums;
using PickMaster.Managers;
using PickMaster.Model;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace PickMaster.MainMenu
{
    public class MainScreenUI : MonoBehaviour
    {
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button startGameLog;
        [SerializeField] private Button upgradeSettingButton;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button upgradeButtonLog;
        [SerializeField] private TMP_Text unlockPrice;
        [SerializeField] private TMP_Text unlockLevelPrice;
        [SerializeField] private Button levelsButton;

        [SerializeField] private Button levelProgressButton;

        [SerializeField] private Button coinsButton;
        
        [SerializeField] private GameObject tutorialStepTwo;

        [SerializeField] private GameObject newLevelsComingSoon;
        
        private Inventory inventory;
        private WindowManager windowManager;
        private Settings settings;
        private EggController egg;
        private AnalyticsController analyticsController;
        private SignalBus signalBus;
        private int uPrice = 0;

        [Inject]
        private void Init(
            Inventory inventory, 
            WindowManager windowManager, 
            Settings settings, 
            EggController egg,
            AnalyticsController analyticsController,
            SignalBus signalBus)
        {
            this.inventory = inventory;
            this.windowManager = windowManager;
            this.settings = settings;
            this.egg = egg;
            this.analyticsController = analyticsController;
            this.signalBus = signalBus;
        }

        private void OnEnable()
        {
            startGameButton.onClick.AddListener(() => { StartLevel(); });
            startGameLog.onClick.AddListener(() =>
            {
                analyticsController.ClickLevelStartLocked(inventory.CurrentSetting);
            });
            
            upgradeButton.onClick.AddListener(() => { UnlockItem(); });
            upgradeButtonLog.onClick.AddListener(() =>
            {
                analyticsController.ClickUnlockItemLocked(inventory.CurrentSetting);
            });
            
            levelsButton.onClick.AddListener(() => { LevelSelect(); });

            levelProgressButton.onClick.AddListener(() =>
            {
                analyticsController.ClickLevelProgress(inventory.CurrentSetting);
                windowManager.OpenWindow(Window.LevelItems);
            });
            
            coinsButton.onClick.AddListener(() =>
            {
                analyticsController.ClickCoins(inventory.Balance);
            });
            
            upgradeSettingButton.onClick.AddListener(() =>
            {
                UnlockSetting();
            });
            
            UpdateUpgradePrice();
            
            tutorialStepTwo.SetActive(inventory.TutorialStep == 3 && inventory.Balance >= uPrice);

            DOTween.SetTweensCapacity(500, 312);
            DOTween.Init();
        }

        private void UpdateUpgradePrice()
        {
            var updateAvailable = inventory.SettingUnlockAvailable();
            Debug.Log($"updateAvailable: {updateAvailable}, CurrentSetting: {inventory.CurrentSetting}");
            if (updateAvailable == SettingUnlockState.UpgradeDone)
            {
                egg.gameObject.SetActive(false);
                upgradeButton.gameObject.SetActive(false);
                upgradeSettingButton.gameObject.SetActive(false);
                startGameButton.interactable = true;
                startGameLog.gameObject.SetActive(false);
                return;
            }
            
            if (updateAvailable == SettingUnlockState.UpgradeAvailable)
            {
                upgradeButton.gameObject.SetActive(false);
                startGameButton.interactable = false;
                upgradeButtonLog.gameObject.SetActive(false);
                startGameLog.gameObject.SetActive(true);
                upgradeSettingButton.interactable = true;
                upgradeSettingButton.gameObject.SetActive(true);
                uPrice = settings.GetSettingConfig(inventory.CurrentSetting).UnlockLevelPrice;
                unlockLevelPrice.text = $"<sprite=0> {BalanceConverter.Convert(uPrice)}";
                return;
            }
            
            if (updateAvailable == SettingUnlockState.UpgradeAvailableButNotAffordable)
            {
                upgradeButton.gameObject.SetActive(false);
                startGameButton.interactable = true;
                upgradeButtonLog.gameObject.SetActive(false);
                startGameLog.gameObject.SetActive(false);
                upgradeSettingButton.interactable = false;
                upgradeSettingButton.gameObject.SetActive(true);
                uPrice = settings.GetSettingConfig(inventory.CurrentSetting).UnlockLevelPrice;
                unlockLevelPrice.text = $"<sprite=0> {BalanceConverter.Convert(uPrice)}";
                return;
            }

            if (updateAvailable == SettingUnlockState.ComingSoon)
            {
                unlockLevelPrice.text = $"";
                upgradeButton.gameObject.SetActive(false);
                newLevelsComingSoon.SetActive(true);
                return;
            }

            upgradeSettingButton.gameObject.SetActive(false);
            upgradeButton.gameObject.SetActive(true);
            
            uPrice = settings.GetSettingConfig(inventory.CurrentSetting)
                .GetUpgradePrice(inventory.GetCurrentSettingLevel());
            
            unlockPrice.text = $"<sprite=0> {BalanceConverter.Convert(uPrice)}";

            #if TUNE_MODE
            upgradeButton.interactable = true;
            startGameButton.interactable = true;
            upgradeButtonLog.gameObject.SetActive(false);
            startGameLog.gameObject.SetActive(false);
            #else
            upgradeButton.interactable = inventory.Balance >= uPrice;
            upgradeButtonLog.gameObject.SetActive(!upgradeButton.interactable);
            
            startGameButton.interactable = !upgradeButton.interactable;
            startGameLog.gameObject.SetActive(upgradeButton.interactable);
            #endif
        }
        
        private void OnDisable()
        {
            startGameButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.RemoveAllListeners();
            levelsButton.onClick.RemoveAllListeners();
            levelProgressButton.onClick.RemoveAllListeners();
            upgradeSettingButton.onClick.RemoveAllListeners();
        }

        private void StartLevel()
        {
            analyticsController.ClickLevelStart(inventory.CurrentSetting);
            Resources.UnloadUnusedAssets();
            SceneManager.LoadScene(SceneNames.GAME_PLAY + "_"+ (inventory.CurrentSetting +1).ToString());
        }

        private void UnlockSetting()
        {
            inventory.Balance -= uPrice;
            var newSettingId = inventory.CurrentSetting + 1;
            inventory.OpenSetting(newSettingId);
            var rollerModels = new List<RollerModel>();

            foreach (var rollerId in inventory.GetSetting(newSettingId).OpenItems)
            {
                rollerModels.Add(settings.GetSettingConfig(newSettingId).GetRoller(rollerId));
            }
            
            egg.Open(rollerModels, () =>
            {
                inventory.CurrentSetting = newSettingId;
                UpdateUpgradePrice();
                signalBus.Fire(new SettingUpgradedSignal(newSettingId));
            });
        }

        private void UnlockItem()
        {
            analyticsController.ClickUnlockItems(inventory.CurrentSetting);
            
            if (inventory.Balance < uPrice)
                return;

            if (inventory.TutorialStep == 3)
            {
                inventory.TutorialStep++;
                tutorialStepTwo.SetActive(false);
            }
            
            inventory.Balance -= uPrice;

            var roller = inventory.LevelUpgrade();
            egg.Open(new List<RollerModel>(){roller}, () =>
            {
                windowManager.OpenWindow(Window.LevelItems);
                analyticsController.ItemUnlock(roller.RollerId, uPrice, inventory.Balance, inventory.CurrentSetting);
                UpdateUpgradePrice();
            });
        }

        private void LevelSelect()
        {
            analyticsController.ClickLevels();
            Resources.UnloadUnusedAssets();
            SceneManager.LoadScene(SceneNames.LEVELS);
        }

        // private void CheckForComingSoon()
        // {
        //     if (settings.GetSettingConfig(inventory.CurrentSetting).Rollers.Count ==
        //         inventory.GetCurrentSetting().OpenItems.Count)
        //     {
        //         upgradeButton.gameObject.SetActive(false);
        //         newLevelsComingSoon.SetActive(true);
        //     }
        // }
    }
}

