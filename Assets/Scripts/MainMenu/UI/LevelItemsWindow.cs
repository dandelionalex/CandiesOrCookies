using PickMaster.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MainMenu.UI
{
    public class LevelItemsWindow : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text levelProgress;

        [SerializeField] 
        private Transform itemContainer;

        [SerializeField] 
        private GameObject itemsPrefab;

        [SerializeField] 
        private Button okButton;

        private Inventory inventory;
        private Settings settings;
        
        [Inject]
        private void Init(Inventory inventory, Settings settings)
        {
            this.inventory = inventory;
            this.settings = settings;
        }
        
        private void OnEnable()
        {
            okButton.onClick.AddListener(OnOkClick);
            var levelModel = inventory.GetCurrentSetting();
            
            foreach (Transform item in itemContainer)
            {
                Destroy(item.gameObject);
            }

            foreach (var itemId in levelModel.OpenItems)
            {
                var go = Instantiate(itemsPrefab, itemContainer);
                var roller = settings.GetSettingConfig(inventory.CurrentSetting).GetRoller(itemId);
                go.GetComponent<LevelItemRenderer>().SetItem(roller);
            }

            var unopened = settings.GetSettingConfig(inventory.CurrentSetting).Rollers.Count - levelModel.OpenItems.Count;
            for (int i = 0; i < unopened; i++)
            {
                var go = Instantiate(itemsPrefab, itemContainer);
                go.GetComponent<LevelItemRenderer>().SetAsEmpty();
            }

            levelProgress.text = $"{levelModel.OpenItems.Count}/{settings.GetSettingConfig(inventory.CurrentSetting).Rollers.Count}";
        }

        private void OnDisable()
        {
            okButton.onClick.RemoveAllListeners();
        }

        private void OnOkClick()
        {
            gameObject.SetActive(false);
        }
    }
}
