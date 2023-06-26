using System;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.UI
{
    public class CollectPanel : MonoBehaviour
    {
        [SerializeField] 
        private Button collectButton;

        private Action onPress;
        
        public void Show(Action onPress)
        {
            this.gameObject.SetActive(true);
            collectButton.onClick.AddListener(OnCollectClick);
            this.onPress = onPress;
        }

        private void OnCollectClick()
        {
            gameObject.SetActive(false);
            onPress.Invoke();
        }

        private void OnDisable()
        {
            collectButton.onClick.RemoveAllListeners();
        }
    }
}