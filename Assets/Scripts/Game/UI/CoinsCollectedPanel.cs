using PickMaster.DI.Signals;
using PickMaster.Managers;
using TMPro;
using UnityEngine;
using Zenject;

namespace PickMaster.Game.UI
{
    public class CoinsCollectedPanel : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text coinsCollectedText;

        [SerializeField]
        private AudioSource coinCollectedSound;
        
        private SignalBus signalBus;
        private Inventory inventory;

        private int previousBalance;
        [Inject]
        private void Init(SignalBus signalBus, Inventory inventory)
        {
            this.signalBus = signalBus;
            this.inventory = inventory;
        }

        private void OnEnable()
        {
            signalBus.Subscribe<GoldCollectedSignal>(OnGoldCollectedSignal);
            SetBalance(0);
        }
        
        private void OnDisable()
        {
            signalBus.Unsubscribe<GoldCollectedSignal>(OnGoldCollectedSignal);
        }

        private void OnGoldCollectedSignal(GoldCollectedSignal gold)
        {
            print("Coin collected");
            coinCollectedSound.Play();
            SetBalance(gold.GoldAmount);
        }

        private void SetBalance(int balance)
        {
            previousBalance += balance;
            coinsCollectedText.text = BalanceConverter.Convert(previousBalance);
        }
    }
}