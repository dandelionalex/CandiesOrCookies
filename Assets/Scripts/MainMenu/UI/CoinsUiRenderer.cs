using PickMaster.Managers;
using TMPro;
using UnityEngine;
using Zenject;

namespace MainMenu.UI
{
    public class CoinsUiRenderer : MonoBehaviour
    {
        private Inventory inventory;
        [SerializeField]
        private TMP_Text balanceText;
    
        [Inject]
        private void Init(Inventory inventory)
        {
            this.inventory = inventory;
        }

        private void OnEnable()
        {
            inventory.BalanceUpdated += OnBalanceUpdated;
            balanceText.text = BalanceConverter.Convert(inventory.Balance);
        }

        private void OnDisable()
        {
            inventory.BalanceUpdated -= OnBalanceUpdated;
        }

        private void OnBalanceUpdated(int value)
        {
            balanceText.text = BalanceConverter.Convert(value);
        }
    }
}

