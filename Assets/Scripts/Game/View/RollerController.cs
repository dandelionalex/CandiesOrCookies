using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using PickMaster.DI.Signals;
using PickMaster.Enums;
using PickMaster.Game.View;
using PickMaster.Logic;
using PickMaster.Managers;
using PickMaster.Model;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;
using DG.Tweening;
using Game.View;

namespace PickMaster.Game.Managers
{
    public class RollerController : MonoBehaviour
    {
        [SerializeField] 
        private Transform spawnPont;
        [SerializeField]
        private GameObject coinPrefab;
        [SerializeField]
        private GameObject ingotPrefab;

        [SerializeField]
        private GameObject ingotDestroyFX;

        [SerializeField]
        private int coinsAmount;

        [SerializeField] 
        private float removeZPos;
        [SerializeField]
        private float removeYPos;

        [SerializeField] 
        private Vector2 initialSpawnRect;

        private SignalBus signalBus;
        private GameLogic gameLogic;

        private Dictionary<string, RollerView> cachedPrefabs = new Dictionary<string, RollerView>();
        private List<Roller> rollers = new List<Roller>();
        private List<Roller> positiveRollers = new List<Roller>();
        private List<Roller> negativeRollers = new List<Roller>();
        
        private List<GameObject> rollersGameObject = new List<GameObject>();
        
        private GameState gameState = GameState.Undefined;
        private DiContainer container;
        private bool alreayInited = false;
        private ConveyorBelt conveyorBelt;
        private Inventory inventory;
        private Settings settings;
        private float startTime = -1;

        [SerializeField] 
        private RollerSpawner spawner;

        [SerializeField]
        private AudioSource spawnGoldIngotSFX;
        [SerializeField]
        private AudioSource bonusGameStartSFX;

        [Inject]
        private void Init(
            SignalBus signalBus, 
            GameLogic gameLogic, 
            DiContainer container,
            ConveyorBelt conveyorBelt,
            Inventory inventory,
            Settings settings
            )
        {
            this.signalBus = signalBus;
            this.gameLogic = gameLogic;
            this.container = container;
            this.conveyorBelt = conveyorBelt;
            this.inventory = inventory;
            this.settings = settings;
        }

        private void OnEnable()
        {
            signalBus.Subscribe<GameStateChangedSignal>(OnGameStateChanged);
            signalBus.Subscribe<TapMadeSignal>(OnFirstTapSignal);
        }
        
        private void OnDisable()
        {
            signalBus.Unsubscribe<GameStateChangedSignal>(OnGameStateChanged);
        }

        private void OnGameStateChanged(GameStateChangedSignal signal)
        {
            gameState = signal.GameState;
            switch (gameState)
            {
                case GameState.Start:
                    StartGame();
                    DestroyAllIngotsLeftAfterGoldenMode();
                    spawner.MakeNormal();
                    break;
                case GameState.Gold:
                    conveyorBelt.ResetSpeed();
                    ConvertAllToCoins();
                    spawner.MakeGolden();
                    break;
                case GameState.Finish:
                    FinishGame();
                    break;
            }
        }

        private bool firstTap = false;

        private void OnFirstTapSignal(TapMadeSignal signal)
        {
            if (inventory.TutorialStep <2 || inventory.TutorialStep == 3)
                return;

            if (firstTap == false)
            {
                firstTap = true;
                return;
            }
            
            signalBus.Unsubscribe<TapMadeSignal>(OnFirstTapSignal);
            startTime = Time.time;
        }

        private void StartGame()
        {
            rollers = gameLogic.GetAvailableItems();
            positiveRollers = rollers.Where(k => k.IsPositive).ToList();
            negativeRollers = rollers.Where(k => !k.IsPositive).ToList();

            rollers.ForEach(x =>
            {
                if(!cachedPrefabs.ContainsKey(x.Model.RollerId))
                    cachedPrefabs.Add(x.Model.RollerId, Resources.Load<GameObject>(x.Model.PrefabName).GetComponent<RollerView>());
            });

            SpawnInitialObjects();
            Debug.Log("start game ");
                
            StartCoroutine(GenObjects());
        }

        private IEnumerator GenObjects()
        {
            while (gameState == GameState.Start)
            {
                GenObject(spawnPont.position, GetRoller() );

                float spawnDelay = GetSpawnDelay();
                spawner.Animate(spawnDelay);
                yield return new WaitForSeconds(spawnDelay);
            }
            
            while (gameState == GameState.Gold)
            {
                GenIngot(spawnPont.position);
                float spawnDelay = GetSpawnDelay();
                spawner.Animate(spawnDelay);
                yield return new WaitForSeconds(spawnDelay);
            }
        }

        private float GetSpawnDelay()
        {
            if(startTime <0)
                return  settings.GetSettingConfig(inventory.CurrentSetting)
                    .GetSpawnDelay(inventory.GetCurrentSettingLevel());
            
            var secondsFromStart = Time.time - startTime;
            
            var spawnDelay = settings.GetSettingConfig(inventory.CurrentSetting)
                .GetSpawnDelay(inventory.GetCurrentSettingLevel());
           
            var spawnDelayDecrease = settings.GetSettingConfig(inventory.CurrentSetting)
                .GetSpawnDelayDecrease(inventory.GetCurrentSettingLevel());
            
            var spawnDelayDecreaseTimeout = settings.GetSettingConfig(inventory.CurrentSetting)
                .GetSpawnDelayDecreaseTimeout(inventory.GetCurrentSettingLevel());
           
            var spawnDelayDecreaseMinimum = settings.GetSettingConfig(inventory.CurrentSetting)
                .GetSpawnDelayDecreaseMinimum(inventory.GetCurrentSettingLevel());
            
            var delay = Math.Max(spawnDelayDecreaseMinimum, spawnDelay - secondsFromStart / spawnDelayDecreaseTimeout * spawnDelayDecrease);
                
            return delay;
        }
        
        private int negativeRollersSpawned = 0;
        
        private void GenObject(Vector3 spawnPoint, Roller roller, bool isStart = false)
        {
            var prefab = cachedPrefabs[roller.Model.RollerId];
            try
            {
                var rollerGO = container.InstantiatePrefab(prefab, new Vector3(spawnPoint.x + Random.Range(-0.2f, 0.2f), spawnPoint.y, spawnPoint.z + Random.Range(-0.2f, 0)), Random.rotation, this.transform);
                if(!isStart)
                    spawner.InitRoller(rollerGO);
                
                rollersGameObject.Add(rollerGO);
                rollerGO.GetComponent<RollerView>().InitWithRoller(roller);
                signalBus.Fire(new RollerSpawnedSignal(roller));
            }
            catch (Exception ex)
            {
                Debug.Log($"exception {ex}" );
            }
        }

        private Roller GetRoller()
        {
            var rollPositiveChance = Random.value < settings.GetSettingConfig(inventory.CurrentSetting)
                .GetRollChance(inventory.GetCurrentSettingLevel());

            if (negativeRollersSpawned > 3)
            {
                negativeRollersSpawned = 0;
                rollPositiveChance = true;
            }
            
            if (rollPositiveChance)
            {
                return positiveRollers[Random.Range(0, positiveRollers.Count)];
            }

            negativeRollersSpawned++;
            return negativeRollers[0];
        }

        private void GenIngot(Vector3 spawnPoint)
        {
            var ingotGO = container.InstantiatePrefab(ingotPrefab, spawnPoint, Random.rotation, this.transform);
            spawner.InitRoller(ingotGO);
            rollersGameObject.Add(ingotGO);
        }
        
        private void Update()
        {
            var rollerToDelete = new List<GameObject>();
            
            foreach (var item in rollersGameObject)
            {
                if ( item.transform.position.z < removeZPos || item.transform.position.y < removeYPos ) 
                {
                    rollerToDelete.Add(item);
                }
            }
            
            foreach (var item in rollerToDelete)
            {
                rollersGameObject.Remove(item);
                Destroy(item);
            }
            rollerToDelete.Clear();
        }

        public void RemoveRollerView(GameObject rollerView)
        {
            rollersGameObject.Remove(rollerView);
            Destroy(rollerView);
        }

        private void ConvertAllToCoins()
        {

            spawnGoldIngotSFX.Play();
            bonusGameStartSFX.Play();

            while (rollersGameObject.Count >0)
            {
                var rollerView = rollersGameObject[0];
                var newIngot = container.InstantiatePrefab(ingotPrefab, rollerView.transform.position, UnityEngine.Random.rotation, this.transform);
                //ingot spawned
                Destroy(newIngot, 15); //TODO: change
                for (int j = 0; j < coinsAmount; j++)
                {
                    var newCoin = container.InstantiatePrefab(coinPrefab, rollerView.transform.position, UnityEngine.Random.rotation, this.transform);
                    newCoin.GetComponent<GoldenCoin>().FlyWithDelay();
                }
                
                RemoveRollerView(rollerView);
            }
        }

        private void DestroyAllIngotsLeftAfterGoldenMode()
        {
            var ingotsLeft = new List<GameObject>();
            ingotsLeft = GameObject.FindGameObjectsWithTag("Ingot").ToList();
            foreach (var ingot in ingotsLeft)
            {
                GameObject newDestroyIngotFX = Instantiate(ingotDestroyFX, ingot.transform.position, Quaternion.identity);
                Destroy(newDestroyIngotFX, 2);
                RemoveRollerView(ingot);
            }
        }
        
        private void FinishGame()
        {
            foreach (var roller in rollersGameObject)
            {
                Destroy(roller);
            }

            rollersGameObject.Clear();
        }
        
        private void SpawnInitialObjects()
        {
            if(alreayInited)
                return;

            alreayInited = true;
            
            for (int i = 0; i < 10; i++)
            {
                var vec = new Vector3(Random.Range(-initialSpawnRect.x, initialSpawnRect.x),0.3f,
                    Random.Range(-initialSpawnRect.y, initialSpawnRect.y));
               
                GenObject( this.transform.position + vec, GetRoller(), true);
            }
        }
    }
}

