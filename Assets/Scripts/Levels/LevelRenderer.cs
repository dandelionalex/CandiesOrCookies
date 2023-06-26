using PickMaster.Managers;
using PickMaster.Model;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace PickMaster.Levels
{
    public class LevelRenderer : MonoBehaviour
    {
        [SerializeField] 
        private Button button;
        [SerializeField] 
        private Image back;
        [SerializeField] 
        private TMP_Text settingProgress;

        [SerializeField] 
        private int settingId = 0;
        
        private Settings settings;  
        private Inventory inventory;
        private AnalyticsController analyticsController;
        
        private Color32 greyBack = new Color32(255, 255, 255,255);
        private Color32 blueBack = new Color32(46, 95, 209,255);
        private Color32 greenBack = new Color32(47, 185, 22,255);
        
        [Inject]
        private void Init(Inventory inventory, Settings settings, AnalyticsController analyticsController)
        {
            this.inventory = inventory;
            this.analyticsController = analyticsController;
            var total = settings.GetSettingConfig(settingId).Rollers.Count;
            var settingModel = inventory.GetSetting(settingId);
            if (settingModel == null)
            {
                button.interactable = false;
                settingProgress.text = $"0/{total}";
                back.color = greyBack;
                return;
            }

            if (total == settingModel.OpenItems.Count)
                back.color = greenBack;
            else
                back.color = blueBack;
            
            var current = settingModel.OpenItems.Count;
            settingProgress.text = $"{current}/{total}";
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveAllListeners();
        }

        private void OnButtonClick()
        {
            inventory.CurrentSetting = settingId;
            analyticsController.ClickLevelsChooseSetting(inventory.CurrentSetting);
            SceneManager.LoadScene(SceneNames.MENU);
        }
    }
    
}

