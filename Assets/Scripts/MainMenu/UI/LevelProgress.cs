using PickMaster.DI.Signals;
using PickMaster.Managers;
using TMPro;
using UnityEngine;
using Zenject;

namespace MainMenu.UI
{
    public class LevelProgress : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text progressLabel;

        [SerializeField] 
        private TMP_Text goldBonus;
        
        [SerializeField] 
        private SlicedFilledImage progressImage;

        [SerializeField] 
        private TMP_Text levelName;
        
        private SignalBus signalBus;
        private Inventory inventory;
        private Settings settings;
        
        [Inject]
        private void Init(SignalBus signalBus, Inventory inventory, Settings settings)
        {
            this.signalBus = signalBus;
            this.inventory = inventory;
            this.settings = settings;
        }

        private void OnEnable()
        {
            signalBus.Subscribe<LevelUpgradedSignal>(OnLevelUpgradedSignal);
            signalBus.Subscribe<SettingUpgradedSignal>(UpdateProgress);
            UpdateProgress();
        }

        private void OnDisable()
        {
            signalBus.Unsubscribe<LevelUpgradedSignal>(OnLevelUpgradedSignal);
            signalBus.Unsubscribe<SettingUpgradedSignal>(UpdateProgress);
        }
        
        private void OnLevelUpgradedSignal(LevelUpgradedSignal signal)
        {
            UpdateProgress();
        }
        
        private void UpdateProgress()
        {
            var settingModel = inventory.GetCurrentSetting();
            var current = inventory.GetCurrentSetting().OpenItems.Count;
            var goldenBonus = settings.GetSettingConfig(settingModel.Id).GetGoldBonusModifier(inventory.GetCurrentSettingLevel());

            var total = settings.GetSettingConfig(settingModel.Id).GetLevelLength();
            progressLabel.text = $"{current}/{total}";
            progressImage.fillAmount = (float) current / total;
            if (goldenBonus > 1)
            {
//                print($"goldenBonus from config = {goldenBonus}, calculated = {((goldenBonus - 1) * 100)}, int = {(int)((goldenBonus - 1) * 100)}");
                goldBonus.text = $"Golden Bonus {Mathf.Round((goldenBonus - 1) * 100)}%";
            }
            else
                goldBonus.text = string.Empty;

            levelName.text = settings.GetSettingConfig(settingModel.Id).SettingName;
        }
    }
}