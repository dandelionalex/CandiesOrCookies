using System;
using System.Collections.Generic;
using System.Linq;
using PickMaster.Managers;
using PickMaster.DI.Signals;
using PickMaster.Enums;
using PickMaster.Game.UI;
using PickMaster.Model;
using UnityEngine;
using Zenject;

namespace PickMaster.Logic
{
    public class GameLogic : IInitializable, IDisposable, IFixedTickable
    {
        private readonly SignalBus signalBus;
        private readonly Settings settings;
        private readonly FinishWindow finishWindow;
        private readonly Inventory inventory;
        private readonly AnalyticsController analyticsController;
        
        private List<Roller> availableItems;
        private float goldModeProgress;

        #region game stat
        
        private int positiveSpawned;
        private int positive1Collected;
        private int positive2Collected;
        private int positive3Collected;
        
        private int negativeSpawned;
        private int negative1Collected;
        private int negative2Collected;
        private int negative3Collected;
        
        private int ingot1Collected;
        private int ingot2Collected;
        private int ingot3Collected;
        
        private int goldCollected;
        #endregion

        private bool firstTap;
        private bool timerActive;
        public float LevelTime;
        public GameState CurrentGameState { get; private set; }



        private GameLogic(
            SignalBus signalBus, 
            Settings settings, 
            FinishWindow finishWindow,
            Inventory inventory,
            AnalyticsController analyticsController)
        {
            this.signalBus = signalBus;
            this.settings = settings;
            this.finishWindow = finishWindow;
            this.inventory = inventory;
            this.analyticsController = analyticsController;
        }

        public List<Roller> GetAvailableItems()
        {
            if(availableItems == null)
            {
                availableItems = new List<Roller>();
                var openedItems = inventory.GetSetting(inventory.CurrentSetting).OpenItems;
                var shuffledItems = openedItems.OrderBy(a => Guid.NewGuid()).ToList();
                
                for (int i= 0; i< shuffledItems.Count; i++)
                {
                    var roller = settings.GetSettingConfig(inventory.CurrentSetting).GetRoller( shuffledItems[i]);
                    availableItems.Add(new Roller(roller, true));
                }

                var bomb = new RollerModel("Bomb", "Bomb", "Bomb", 0f);
                availableItems.Add(new Roller(bomb, false));
            }
            
            return availableItems;
        }

        public void Initialize()
        {
            signalBus.Subscribe<TapMadeSignal>(OnTapSignal);
            //signalBus.Subscribe<HandShownSignal>(OnHandShownSignal);
            signalBus.Subscribe<RollerCollectedSignal>(OnRollerCollectedSignal);
            signalBus.Subscribe<GoldCollectedSignal>(OnGoldCollectedSignal);
            signalBus.Subscribe<RollerSpawnedSignal>(OnRollerSpawnedSignal);
            signalBus.Subscribe<IngotCollectedSignal>(OnIngotCollectedSignal);
            Vibration.Init();
            analyticsController.LevelStart(inventory.CurrentSetting);
            signalBus.Fire(new GameStateChangedSignal(GameState.Start));
            CurrentGameState = GameState.Start;
        }

        public void Dispose()
        {
            //signalBus.Unsubscribe<HandShownSignal>(OnHandShownSignal);
            signalBus.Unsubscribe<RollerCollectedSignal>(OnRollerCollectedSignal);
            signalBus.Unsubscribe<GoldCollectedSignal>(OnGoldCollectedSignal);
            signalBus.Unsubscribe<IngotCollectedSignal>(OnIngotCollectedSignal);
            signalBus.Unsubscribe<RollerSpawnedSignal>(OnRollerSpawnedSignal);
        }

        private void OnIngotCollectedSignal(IngotCollectedSignal signal)
        {
            if(signal.CollectPosition.z < 0.5)
                ingot3Collected++;
            else if(signal.CollectPosition.z < 4)
                ingot2Collected++;
            else
                ingot1Collected++;
        }

        private void OnGoldCollectedSignal(GoldCollectedSignal signal)
        {
            goldCollected += signal.GoldAmount;
            finishWindow.UpdateGold(goldCollected);
        }

        private void OnRollerSpawnedSignal(RollerSpawnedSignal signal)
        {
            if (signal.Roller.IsPositive)
                positiveSpawned++;
            else
                negativeSpawned++;
        }
        
        private void OnRollerCollectedSignal(RollerCollectedSignal signal)
        {
            if (signal.Roller.IsPositive)
            {
                if(signal.CollectPosition.z < 0.5)
                    positive3Collected++;
                else if(signal.CollectPosition.z < 4)
                    positive2Collected++;
                else
                    positive1Collected++;
            }
            else
            {
                if(signal.CollectPosition.z < 0.5)
                    negative3Collected++;
                else if(signal.CollectPosition.z < 4)
                    negative2Collected++;
                else
                    negative1Collected++;
                
                FinishLevel();
                return;
            }

            var rollerWeight = signal.Roller.Model.LevelProgress * settings.GetSettingConfig(inventory.CurrentSetting).GetRollerCoefficient(inventory.GetCurrentSettingLevel());

            var progressValue = signal.Roller.IsPositive
                ? rollerWeight
                : rollerWeight*0;
            
            goldModeProgress += progressValue;



            if (goldModeProgress >= 1)
                GoldenMode();
            if (goldModeProgress < 0)
                goldModeProgress = 0;
            
            signalBus.Fire(new LevelGoldProgressSignal(goldModeProgress, progressValue));
        }
        
        // private void OnHandShownSignal(HandShownSignal signal)
        // {
        //
        //     signalBus.Fire(new GameStateChangedSignal(GameState.Start));
        // }

        private void OnTapSignal(TapMadeSignal signal)
        {
            if(inventory.TutorialStep <3)
                return;
            
            if (firstTap == false)
            {
                firstTap = true;
                return;
            }
            
            timerActive = true;
            signalBus.Unsubscribe<TapMadeSignal>(OnTapSignal);
        }
        
        private void GoldenMode()
        {
            CurrentGameState = GameState.Gold;
            signalBus.Fire(new GameStateChangedSignal(GameState.Gold));
        }

        private float timeInGoldState = 0;
        
        public void FixedTick()
        {
            if(CurrentGameState == GameState.Finish)
                return;
            
            if (CurrentGameState == GameState.Gold)
            {
                timeInGoldState += Time.fixedDeltaTime;
                goldModeProgress = 1 - timeInGoldState / settings.GetSettingConfig(inventory.CurrentSetting).GetGoldenRainDuration(inventory.GetCurrentSettingLevel());
                signalBus.Fire(new LevelGoldProgressSignal(goldModeProgress, 0.05f));

                if (timeInGoldState > settings.GetSettingConfig(inventory.CurrentSetting).GetGoldenRainDuration(inventory.GetCurrentSettingLevel()))
                {
                    CurrentGameState = GameState.Start;
                    signalBus.Fire(new GameStateChangedSignal(GameState.Start));
                    timeInGoldState = 0;
                }
                return;
            }

            if (timerActive)
            {
                LevelTime += Time.fixedDeltaTime;
            }

            if (LevelTime >= settings.GetSettingConfig(inventory.CurrentSetting).GetLevelDuration(inventory.GetCurrentSettingLevel()) 
                && CurrentGameState != GameState.Finish)
            {
                FinishLevel();
            }
        }

        public void FinishLevel()
        {
            inventory.Balance += goldCollected;
            CurrentGameState = GameState.Finish;
            signalBus.Fire(new GameStateChangedSignal(GameState.Finish));

            var statistics = new Dictionary<string, int>()
            {
                {"positive_spawned", positiveSpawned}, 
                {"positive_collected_1", positive1Collected}, 
                {"positive_collected_2", positive2Collected}, 
                {"positive_collected_3", positive3Collected}, 
                {"negative_spawned", negativeSpawned},
                {"negative_collected_1", negative1Collected}, 
                {"negative_collected_2", negative2Collected}, 
                {"negative_collected_3", negative3Collected}, 
                {"coins_spawned", goldCollected}, 
                {"coins_collected_1", ingot1Collected}, 
                {"coins_collected_2", ingot2Collected}, 
                {"coins_collected_3", ingot3Collected}, 
                {"duration", (int) LevelTime},
                {"setting_id", inventory.CurrentSetting},
            };
                
            analyticsController.LevelComplete(statistics);
            inventory.Round++;
            timerActive = false;
            finishWindow.Open(goldCollected, analyticsController);
        }
    }
}

