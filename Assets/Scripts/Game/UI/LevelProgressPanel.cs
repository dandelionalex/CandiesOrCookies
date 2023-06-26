using PickMaster.DI.Signals;
using PickMaster.Enums;
using PickMaster.Logic;
using PickMaster.Managers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PickMaster.Game.UI
{
    public class LevelProgressPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject arrow;
        [SerializeField]
        private Image redSectorImage;
        [SerializeField]
        private Slider levelProgressSlider;
        [SerializeField]
        private GameObject goldenCoin;
        [SerializeField]
        private GameObject grayCoin;

        [SerializeField]
        private AudioSource progressIncreasedSFX;

        private bool isGoldenMode = false;
        
        private float timer = 0.0f;
        private float levelProgress;
        
        private SignalBus signalBus;
        private Settings settings;
        private GameLogic gameLogic;
        private Inventory inventory;
            
        [Inject]
        private void Init(
            SignalBus signalBus, 
            Settings settings,
            GameLogic gameLogic,
            Inventory inventory)
        {
            this.signalBus = signalBus;
            this.settings = settings;
            this.gameLogic = gameLogic;
            this.inventory = inventory;
        }

        private void OnEnable()
        {
            signalBus.Subscribe<LevelGoldProgressSignal>(OnRollerCollectedSignal);
            signalBus.Subscribe<GameStateChangedSignal>(OnGameStateChangedSignal);
            levelProgressSlider.value = levelProgress;
        }
        
        private void OnDisable()
        {
            signalBus.Unsubscribe<LevelGoldProgressSignal>(OnRollerCollectedSignal);
            signalBus.Unsubscribe<GameStateChangedSignal>(OnGameStateChangedSignal);
        }

        private void OnRollerCollectedSignal(LevelGoldProgressSignal signal)
        {
            levelProgressSlider.value = signal.Total;
            if(!isGoldenMode && signal.Delta > 0)
            {
                progressIncreasedSFX.pitch = 2 + signal.Total * 2;
                progressIncreasedSFX.Play();
            }

        }
        
        private void OnGameStateChangedSignal(GameStateChangedSignal signal)
        {
            if (signal.GameState == GameState.Gold)
            {
                goldenCoin.SetActive(true);
                grayCoin.SetActive(false);
                isGoldenMode = true;
            }
            else if (signal.GameState == GameState.Start)
            {
                goldenCoin.SetActive(false);
                grayCoin.SetActive(true);
                isGoldenMode = false;
            }
        }

        private void FixedUpdate()
        {
            if (gameLogic.CurrentGameState == GameState.Finish)
            {
                redSectorImage.fillAmount = 0;
                arrow.transform.eulerAngles = new Vector3(0, 0, -360);
                return;
            }
                
            var levelCompletion = gameLogic.LevelTime / settings.GetSettingConfig(inventory.CurrentSetting).GetLevelDuration(inventory.GetCurrentSettingLevel());
            var arrowRotation = -360 * levelCompletion;
            arrow.transform.eulerAngles = new Vector3(0, 0, arrowRotation);
            
            redSectorImage.fillAmount = 1 - levelCompletion;

            var tempColor = redSectorImage.color;
            tempColor.a = levelCompletion;
            redSectorImage.color = tempColor;
        }
    }
}
