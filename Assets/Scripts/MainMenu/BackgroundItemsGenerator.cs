using System.Collections;
using System.Collections.Generic;
using PickMaster.DI.Signals;
using PickMaster.Managers;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace PickMaster.MainMenu
{
    public class BackgroundItemsGenerator : MonoBehaviour
    {
        public float generationWidth;
        public float delay;
    
        private Inventory inventory;
        private Settings settings;
        private List<string> openedItems = new List<string>();
        private Dictionary<string, GameObject> cachedPrefabs = new Dictionary<string, GameObject>();
        private SignalBus signalBus;
            
        [Inject]
        private void Init(Inventory inventory, Settings settings, SignalBus signalBus)
        {
            this.inventory = inventory;
            this.settings = settings;
            this.signalBus = signalBus;
        }

        private void OnEnable()
        {
            signalBus.Subscribe<SettingUpgradedSignal>(OnSettingUpgraded);
        }
        
        private void OnDisable()
        {
            signalBus.Unsubscribe<SettingUpgradedSignal>(OnSettingUpgraded);
        }

        private void OnSettingUpgraded()
        {
            StopAllCoroutines();
            cachedPrefabs.Clear();
            Start();
        }
        
        private void Start()
        {
            StartCoroutine(SpawnObjects());
        }
    
        private IEnumerator SpawnObjects()
        {
            openedItems = inventory.GetSetting(inventory.CurrentSetting).OpenItems;
            settings.GetSettingConfig(inventory.CurrentSetting).Rollers.ForEach(roller =>
            {
                if(!openedItems.Contains(roller.RollerId))
                    return;
            
                if(!cachedPrefabs.ContainsKey(roller.RollerId))
                    cachedPrefabs.Add(roller.RollerId, Resources.Load<GameObject>(roller.PrefabName));
            });
        
            while (true)
            {
                var posX = transform.position.x + Random.Range(-1 * generationWidth, generationWidth);
                var pos = new Vector3(posX, transform.position.y, transform.position.z);
                var item = openedItems[Random.Range(0, cachedPrefabs.Count)];
                var newItem = Instantiate(cachedPrefabs[item], pos, Random.rotation, transform);

                var rb = newItem.GetComponent<Rigidbody>();
                rb.drag = 5f;

                Destroy(newItem, 10);
                yield return new WaitForSeconds(delay);
            }
        }
    }
}
