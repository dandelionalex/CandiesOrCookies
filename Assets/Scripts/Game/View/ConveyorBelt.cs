using System.Collections.Generic;
using PickMaster.DI.Signals;
using UnityEngine;
using Zenject;

namespace PickMaster.Game.View
{
    public class ConveyorBelt : MonoBehaviour
    {
        [SerializeField] 
        private float speed;
        [SerializeField] 
        private Vector3 direction;
        [SerializeField] 
        private List<GameObject> onBelt;
        [SerializeField] 
        private GameObject windFX;

        private float startingSpeed;
        private SignalBus signalBus;

        [Inject]
        private void Init(SignalBus signalBus)
        {
            this.signalBus = signalBus;
        }

        private void OnEnable()
        {
            signalBus.Subscribe<RollerCollectedSignal>(OnRollerCollected);
        }

        private void OnDisable()
        {
            signalBus.Unsubscribe<RollerCollectedSignal>(OnRollerCollected);
        }

        private void OnRollerCollected(RollerCollectedSignal signal)
        {
            //IncreaseSpeed();
        }

        private void Start()
        {
            startingSpeed = speed;
            windFX.SetActive(false);
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < onBelt.Count; i++)
            {
                if (onBelt[i] == null)
                {
                    onBelt.RemoveAt(i);
                }
                else
                {
                    onBelt[i].GetComponent<Rigidbody>().velocity = speed * direction * Time.deltaTime;
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            onBelt.Add(collision.gameObject);
        }

        private void OnCollisionExit(Collision collision)
        {
            onBelt.Remove(collision.gameObject);
        }

        public void IncreaseSpeed()
        {
            if (speed < startingSpeed * 2)
            {
                speed *= 1.1f;

                if (speed > startingSpeed * 1.5)
                {
                    windFX.SetActive(true);
                }
                else
                {
                    windFX.SetActive(false);
                }
            }
        }

        public void ResetSpeed()
        {
            speed = startingSpeed;
            windFX.SetActive(false);
        }
    }
}