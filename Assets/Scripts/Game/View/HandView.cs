using System.Collections;
using System.Linq;
using DG.Tweening;
using PickMaster.DI.Signals;
using PickMaster.Enums;
using PickMaster.Logic;
using PickMaster.Managers;
using UnityEngine;
using Zenject;

namespace PickMaster.Game.View
{
    public class HandView : MonoBehaviour
    {
        [SerializeField] 
        private DOTweenAnimation doTweenAnimation;

        [SerializeField] 
        private Transform[] spawnPoints;

        [SerializeField] 
        private Transform goldSpawnPoint;
        
        private GameLogic gameLogic;
        private SignalBus signalBus;

        [SerializeField]
        private GameObject goldenModeFX;

        [SerializeField]
        private GameObject normalModeFX;

        [SerializeField]
        private GameObject goldenRainFX;

        [SerializeField] 
        private GameObject handGameObject;
        
        private Inventory inventory;

        [Inject]
        private void Init(GameLogic gameLogic, SignalBus signalBus, Inventory inventory)
        {
            this.gameLogic = gameLogic;
            this.signalBus = signalBus;
            this.inventory = inventory;
        }

        private void OnEnable()
        {
            var rollersToCollect= gameLogic.GetAvailableItems().Where( x => x.IsPositive).ToList();
            for (var i = 0; i < rollersToCollect.Count(); i++)
            {
               var prefab =  Resources.Load<GameObject>(rollersToCollect[i].Model.PrefabName);
               var go= Instantiate(prefab);
               go.transform.SetParent(spawnPoints[i], false);
               Destroy(go.GetComponent<Rigidbody>());
               var colliders = go.GetComponents<Collider>();
               foreach (var collider in colliders)
               {
                   Destroy(collider); 
               }
               
               Destroy(go.GetComponent<RollerView>());
            }
            
            doTweenAnimation.onComplete.AddListener(OnAnimationComplete);
            signalBus.Subscribe<GameStateChangedSignal>(OnGameStateChangedSignal);
        }
        
        private void OnDisable()
        {
            doTweenAnimation.onComplete.RemoveAllListeners();
            signalBus.TryUnsubscribe<GameStateChangedSignal>(OnGameStateChangedSignal);
        }

        private void OnGameStateChangedSignal(GameStateChangedSignal signal)
        {
            if (signal.GameState == GameState.Gold)
            {
                foreach (var spawnPoint in spawnPoints)
                {
                    spawnPoint.gameObject.SetActive(false);
                }

                print("GOLDEN mode FX");
                goldSpawnPoint.gameObject.SetActive(true);
                GameObject newGoldenModeFX = Instantiate(goldenModeFX, goldSpawnPoint.transform);
                Destroy(newGoldenModeFX, 3);
                goldenRainFX.SetActive(true);
                
            }
            else if (signal.GameState == GameState.Start)
            {
                goldSpawnPoint.gameObject.SetActive(false);
                
                foreach (var spawnPoint in spawnPoints)
                {
                    spawnPoint.gameObject.SetActive(true);
                    GameObject newNormalModeFX = Instantiate(normalModeFX, spawnPoint.transform);
                    Destroy(newNormalModeFX, 3);
                }

                print("Normal mode FX");
                goldenRainFX.SetActive(false);
                
            }
        }

        private void OnAnimationComplete()
        {
            signalBus.Fire<HandShownSignal>();   
        }
    }
}