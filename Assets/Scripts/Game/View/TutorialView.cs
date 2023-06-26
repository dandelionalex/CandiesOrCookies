using PickMaster.DI.Signals;
using PickMaster.Managers;
using UnityEngine;
using Zenject;

namespace PickMaster.Game.View
{
    public class TutorialView : MonoBehaviour
    {
        [SerializeField]
        private GameObject tutorialGameplayStep0;
        
        [SerializeField]
        private GameObject tutorialGameplayStep1;
        
        [SerializeField]
        private GameObject tutorialGameplayStep2;
        
        [SerializeField] 
        private GameObject tapToPlayGO;
        
        [SerializeField] 
        private Transform placeHolder;
        
        private Inventory inventory;
        private SignalBus signalBus;
        private GameObject currentGO;
        
        [Inject]
        private void Init(Inventory inventory, SignalBus signalBus)
        {
            this.inventory = inventory;
            this.signalBus = signalBus;
        }
        
        private void OnEnable()
        {
            signalBus.Subscribe<TapMadeSignal>(OnTapSignal);
            if (inventory.TutorialStep > 2)
            {
                tapToPlayGO.SetActive(true);
                return;
            }
            
            if (inventory.TutorialStep == 0)
                currentGO = Instantiate(tutorialGameplayStep0, placeHolder);
            else if(inventory.TutorialStep == 1)
                currentGO = Instantiate(tutorialGameplayStep1, placeHolder);
            else if(inventory.TutorialStep == 2)
                currentGO = Instantiate(tutorialGameplayStep2, placeHolder);
        }

        private void OnDisable()
        {
            signalBus.TryUnsubscribe<TapMadeSignal>(OnTapSignal);
        }

        private void OnTapSignal(TapMadeSignal signal)
        {
            Debug.Log($"OnFirstTapSignal {inventory.TutorialStep}");
            if (inventory.TutorialStep == 0)
            {
                Destroy(currentGO);
                currentGO = Instantiate(tutorialGameplayStep1, placeHolder);
                inventory.TutorialStep++;
                return;
            }
            
            if (inventory.TutorialStep == 1)
            {
                Destroy(currentGO);
                currentGO = Instantiate(tutorialGameplayStep2, placeHolder);
                inventory.TutorialStep++;
                return;
            }

            if (inventory.TutorialStep == 2)
            {
                Destroy(currentGO);
                inventory.TutorialStep++;
                tapToPlayGO.SetActive(true);
                return;
            }
            
            tapToPlayGO.SetActive(false);
            signalBus.TryUnsubscribe<TapMadeSignal>(OnTapSignal);
        }
    }
}
