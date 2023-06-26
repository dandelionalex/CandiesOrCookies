using System.Collections.Generic;
using System.Linq;
using PickMaster.DI.Signals;
using PickMaster.Game.Managers;
using PickMaster.Game.View;
using PickMaster.Managers;
using UnityEngine;
using Zenject;

namespace PickMaster.Logic
{
    public class TouchLogic : MonoBehaviour
    {
        [SerializeField] 
        private GameObject successfulClickFX;
        [SerializeField] 
        private GameObject falseClickFX;
        [SerializeField]
        private int coinsAmount;
        [SerializeField]
        private GameObject coinPrefab;

        [SerializeField]
        private AudioSource negativeItemCollectedSFX;
        [SerializeField]
        private AudioSource positiveItemCollectedSFX;
        [SerializeField]
        private AudioSource goldIngotCollectedSFX;
        
        private SignalBus signalBus;
        private RollerController _rollerController;
        private DiContainer container;
        private Inventory inventory;
        private Settings settings;
        
        [Inject]
        private void Init(
            SignalBus signalBus, 
            RollerController rollerController, 
            DiContainer container,
            Inventory inventory,
            Settings settings)
        {
            this.signalBus = signalBus;
            this._rollerController = rollerController;
            this.container = container;
            this.inventory = inventory;
            this.settings = settings;
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                signalBus.Fire<TapMadeSignal>();
            
            if (!Input.GetMouseButton(0)) 
                return;
            
            if(inventory.TutorialStep < 3)
                return;
                
            #if UNITY_EDITOR
            var mousePos = Input.mousePosition;
            var hit = CheckHit(mousePos);
            if(hit != null) 
                DoHitAction(hit);
            #else
            for (int i = 0; i < Input.touchCount; i++)
            {
                var mousePos = Input.touches[i].position;
                var hit = CheckHit(mousePos);
                if(hit != null) 
                    DoHitAction(hit);
            }
            #endif
        }

        private Collider CheckHit(Vector3 mousePos)
        {
            var yPosNorm = 1-mousePos.y / Screen.height;
            var rays = new List<Ray>();
            var shift = 20 + yPosNorm * 36;
            
            rays.Add(Camera.main.ScreenPointToRay(mousePos) ); 
            rays.Add(Camera.main.ScreenPointToRay(new Vector3(mousePos.x+shift, mousePos.y, mousePos.z)) ); 
            rays.Add(Camera.main.ScreenPointToRay(new Vector3(mousePos.x+shift, mousePos.y+shift, mousePos.z)) ); 
            rays.Add(Camera.main.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y+shift, mousePos.z)) ); 
            rays.Add(Camera.main.ScreenPointToRay(new Vector3(mousePos.x-shift, mousePos.y+shift, mousePos.z)) ); 
            rays.Add(Camera.main.ScreenPointToRay(new Vector3(mousePos.x-shift, mousePos.y, mousePos.z)) ); 
            rays.Add(Camera.main.ScreenPointToRay(new Vector3(mousePos.x+shift, mousePos.y-shift, mousePos.z)) ); 
            rays.Add(Camera.main.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y-shift, mousePos.z)) ); 
            rays.Add(Camera.main.ScreenPointToRay(new Vector3(mousePos.x+shift, mousePos.y-shift, mousePos.z)) );
            
            var dic = new Dictionary<string, ColliderCount>();
            foreach (var ray in rays)
            {
                RaycastHit raycastHit;
                if(Physics.Raycast(ray, out raycastHit, 90,LayerMask.GetMask("Roller")))
                {
                    if (dic.ContainsKey(raycastHit.collider.tag))
                    {
                        dic[raycastHit.collider.tag].Count++;
                    }
                    else
                    {
                        dic[raycastHit.collider.tag] = new ColliderCount()
                        {
                            Collider = raycastHit.collider
                        };
                    }
                }
            }
            
            if(dic.Count ==0)
                return null;
            
            //Debug.Log($"collided with: {resultCollider.Value.Collider.name}, touch count: {max}");
            // foreach (var ray in rays)
            // {
            //     Debug.DrawRay(ray.origin, ray.direction*10);
            // }
            // EditorApplication.isPaused = true;
            
            var max = dic.Max(c => c.Value.Count);
            var resultCollider = dic.FirstOrDefault(c => c.Value.Count == max);
            return resultCollider.Value.Collider;
        }

        private void DoHitAction(Collider hit)
        {
            if (hit.CompareTag("Roller"))
            {
                print($"Roller hit {hit.name}");

                var rollerView = hit.GetComponent<RollerView>();
                if (rollerView.IsPositive)
                {
                    var pos = hit.transform.position;
                    _rollerController.RemoveRollerView(hit.gameObject);
                    var fx = Instantiate(successfulClickFX, pos, Quaternion.identity);
                    Destroy(fx, 1);
                    positiveItemCollectedSFX.pitch = Random.Range(0.8f, 1.2f);
                    positiveItemCollectedSFX.Play();
                    Vibration.Vibrate(30);
                    signalBus.Fire(new RollerCollectedSignal(rollerView.Roller, pos));
                }
                else
                {
                    Vibration.Vibrate(300);
                    var pos = hit.transform.position;
                    _rollerController.RemoveRollerView(hit.gameObject);
                    var fx = Instantiate(falseClickFX, pos, Quaternion.identity);
                    Destroy(fx, 3);
                    negativeItemCollectedSFX.Play();
                    signalBus.Fire(new RollerCollectedSignal(rollerView.Roller, pos));
                    //conveyorBelt.ResetSpeed();
                }
            }
            else if (hit.CompareTag("Ingot"))
            {
                var settingModel = inventory.GetCurrentSetting();
                var goldBonus = settings.GetSettingConfig(settingModel.Id).GetGoldBonusModifier(inventory.GetCurrentSettingLevel());
                Debug.Log($"gold bonus: {goldBonus}");
                var coins = coinsAmount + coinsAmount*goldBonus;
                for (int j = 0; j < coins; j++)
                {
                    var newCoin = container.InstantiatePrefab(coinPrefab, hit.transform.position, UnityEngine.Random.rotation, this.transform);
                    newCoin.GetComponent<GoldenCoin>().FlyWithDelay();
                }
                goldIngotCollectedSFX.pitch = Random.Range(0.8f, 1.2f);
                goldIngotCollectedSFX.Play();
                signalBus.Fire(new IngotCollectedSignal( hit.transform.position ));
                _rollerController.RemoveRollerView(hit.gameObject);
            }
        }
        
        private class ColliderCount
        {
            public Collider Collider;
            public int Count;
        }
    }
}