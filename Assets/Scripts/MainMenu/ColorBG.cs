using PickMaster.DI.Signals;
using PickMaster.Managers;
using UnityEngine;
using Zenject;

namespace PickMaster.MainMenu
{
    public class ColorBG : MonoBehaviour
    {
        [SerializeField] 
        private MeshRenderer meshRenderer;

        [SerializeField] 
        private Material[] materials;
        
        private SignalBus signalBus;
        private Inventory inventory;
        
        [Inject]
        private void Init(SignalBus signalBus, Inventory inventory)
        {
            this.signalBus = signalBus;
            this.inventory = inventory;
        }

        private void OnEnable()
        {
            signalBus.Subscribe<SettingUpgradedSignal>(OnSettingsUpgrade);
            meshRenderer.material = materials[inventory.CurrentSetting];
        }
        
        private void OnDisable()
        {
            signalBus.Unsubscribe<SettingUpgradedSignal>(OnSettingsUpgrade);
        }

        private void OnSettingsUpgrade(SettingUpgradedSignal signal)
        {
            meshRenderer.material = materials[inventory.CurrentSetting];
        }
    }
}