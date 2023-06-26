using System;
using System.Collections.Generic;
using MainMenu.UI;
using PickMaster.DI.Signals;
using PickMaster.Managers;
using PickMaster.Model;
using UnityEngine;
using Zenject;

namespace PickMaster.MainMenu
{
    public class EggController : MonoBehaviour
    {
        [SerializeField] 
        private Egg eggPrefab;

        [SerializeField] 
        private GameObject staticEggGO;
        
        [SerializeField] 
        private GameObject[] hideElements;
        
        [SerializeField]
        public CollectPanel collectPanel;

        [SerializeField] 
        public Material goldMaterial;

        [SerializeField] 
        private Material[] levelsMaterials;

        [SerializeField] 
        private GameObject eggBg;

        [SerializeField]
        private AudioSource eggChargeUpSound;
        
        private Action eggAnimationFinished;
        private Egg egg;
        private Inventory inventory;
        private Settings settings;
        private SignalBus signalBus;
        [Inject]
        private void Init( Inventory inventory, Settings settings, SignalBus signalBus)
        {
            this.inventory = inventory;
            this.settings = settings;
            this.signalBus = signalBus;
            UpdateColor();
        }

        private void OnEnable()
        {
            signalBus.Subscribe<SettingUpgradedSignal>(OnSettingUpdateSignal);
            signalBus.Subscribe<LevelUpgradedSignal>(OnLevelUpdateSignal);
            
        }
        
        private void OnDisable()
        {
            signalBus.Unsubscribe<SettingUpgradedSignal>(OnSettingUpdateSignal);
            signalBus.Unsubscribe<LevelUpgradedSignal>(OnLevelUpdateSignal);
        }

        private void OnSettingUpdateSignal()
        {
            UpdateColor();
        }
        
        private void OnLevelUpdateSignal()
        {
            UpdateColor();
        }

        private void SetEggActive(bool value)
        {
            staticEggGO.SetActive(value);
        }
        
        public void Open(List<RollerModel> rollerToOpen, Action eggAnimationFinished)
        {
            this.eggAnimationFinished = eggAnimationFinished;
            HideInterface(true);
            egg = Instantiate(eggPrefab);
            eggChargeUpSound.Play();
            if (rollerToOpen.Count > 1)
                ChangeMaterial(egg.gameObject, goldMaterial);
            else
                ChangeMaterial(egg.gameObject, levelsMaterials[inventory.CurrentSetting]);

            egg.Open(rollerToOpen, AnimationFinished);
        }

        private void AnimationFinished()
        {            
            collectPanel.Show(OnCollectPress);
        }

        private bool CheckForSettingUpgrade()
        {
            var settingModel = inventory.GetCurrentSetting();
            var current = inventory.GetCurrentSetting().OpenItems.Count;
            var total = settings.GetSettingConfig(settingModel.Id).GetLevelLength();
            return current == total;
        }

        private void OnCollectPress()
        {
            HideInterface(false);
            Destroy(egg.gameObject);
            eggAnimationFinished.Invoke();
        }
        
        private void HideInterface(bool isHide)
        {
            foreach (var child in hideElements)
            {
                child.SetActive(!isHide);
            }
        }

        private void ChangeMaterial(GameObject go, Material material)
        {
           var meshes= go.GetComponentsInChildren<MeshRenderer>();
           foreach (var meshRenderer in meshes)
           {
               meshRenderer.material = material;
           }
        }

        private void UpdateColor()
        {
            if(CheckForSettingUpgrade())
                ChangeMaterial(eggBg, goldMaterial);
            else
                ChangeMaterial(eggBg, levelsMaterials[inventory.CurrentSetting]);
        }
    }
}