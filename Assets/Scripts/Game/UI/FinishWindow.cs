using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Zenject;

namespace PickMaster.Game.UI
{
    public class FinishWindow : MonoBehaviour
    {
        [SerializeField] 
        private Button okButton;

        [SerializeField] 
        private TMP_Text gold;

        // [SerializeField] 
        // private StatRenderer statPrefab;
        //
        // [SerializeField] 
        // private Transform statContainer;
        
        [SerializeField] 
        private RectTransform containerTransform;

        [SerializeField]
        private AudioSource levelCompleteSFX;
        [SerializeField]
        private AudioSource applaudsSound;

        private AnalyticsController analyticsController;
        
        private void OnEnable()
        {
            okButton.onClick.AddListener(OnOkClick);
        }

        private void OnDisable()
        {
            okButton.onClick.RemoveAllListeners();
        }

        private void OnOkClick()
        {
            analyticsController.ClickClaim();
            SceneManager.LoadScene("Menu");
        }

        public void Open(
            int goldAmount,
            AnalyticsController analyticsController
            // int positiveSpawned, 
            // int negativeSpawned, 
            // int missClick, 
            // int positiveCollected)
            )
        {   
            gold.text = $"<sprite=0> {goldAmount}";
            this.analyticsController = analyticsController;
            containerTransform.gameObject.SetActive(true);
            containerTransform.localPosition = new Vector3(-800, 0, 0);
            containerTransform.DOLocalMoveX(0, 0.5f);

            levelCompleteSFX.Play();
            applaudsSound.Play();

            // Instantiate(statPrefab, statContainer).SetValue("PositiveSpawned:",positiveSpawned.ToString());
            // Instantiate(statPrefab, statContainer).SetValue("NegativeSpawned:",negativeSpawned.ToString());
            // Instantiate(statPrefab, statContainer).SetValue("MissClick:",missClick.ToString());
            // Instantiate(statPrefab, statContainer).SetValue("PositiveCollected:",positiveCollected.ToString());
        }

        public void UpdateGold(int goldAmount)
        {
            gold.text = $"<sprite=0> {goldAmount}";
        }
    }
}