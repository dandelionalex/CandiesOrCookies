using System;
using System.Collections;
using System.Collections.Generic;
using PickMaster.Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PickMaster.MainMenu
{
    public class Egg : MonoBehaviour
    {
        public float explosionForce = 300;
        public float shakeDuration = 3;
        public float shakeAmount;

       
        public GameObject shakeFX;
        public GameObject explodeFX;

        private Vector3 originalPosition;
        private float originalDuration;
        private Action animationFinished;

        private void Start()
        {
            originalPosition = transform.localPosition;
            originalDuration = shakeDuration;
        }

        private void Explode()
        {
            foreach (Transform child in transform)
            {
                var rb = child.gameObject.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.AddExplosionForce(explosionForce, transform.position, 5);
            }
        }

        public void Open(List<RollerModel> rollerToOpen, Action animationFinished)
        {
            this.animationFinished = animationFinished;
            var GOs = new List<GameObject>();
            foreach (var rollerModel in rollerToOpen)
            {
                GOs.Add(Resources.Load<GameObject>(rollerModel.PrefabName));
            }
            StartCoroutine(OpenSequence(GOs));
        }
        
        private IEnumerator OpenSequence(List<GameObject> rollersToOpen)
        {
            yield return new WaitForEndOfFrame();

            StartCoroutine(Shake());

            var newshakeFX = Instantiate(shakeFX, transform);
            Destroy(newshakeFX, shakeDuration);

            yield return new WaitForSeconds(shakeDuration);
            StopCoroutine(Shake());
            Explode();

            var newExplodeFX = Instantiate(explodeFX, transform);
            Destroy(newExplodeFX, 2);

            var i = 0;
            foreach (var rollerPrefab in rollersToOpen)
            {
                var openedRoller = Instantiate(rollerPrefab, transform);
                Destroy(openedRoller.GetComponent<Rigidbody>());
                Destroy(openedRoller.GetComponent<Collider>());
                
                if (rollersToOpen.Count > 1)
                {
                    openedRoller.transform.localPosition = GetPos(i);
                    i++;
                }
            }

            yield return new WaitForSeconds(1);
            shakeDuration = originalDuration;
            animationFinished.Invoke();
        }

        private Vector3 GetPos(int i)
        {
            switch (i)
            {
                case 0:
                    return new Vector3(0, 0, 0.2f);
                case 1:
                    return new Vector3(-0.2f, 0, 0);
                case 2:
                    return new Vector3(0, 0, -0.2f);
                case 3:
                    return new Vector3(0.2f, 0, 0);
            }
            return new Vector3(0, 0, 0);  
        }
        
        private IEnumerator Shake()
        {
            while (shakeDuration > 0)
            {
                shakeDuration -= Time.fixedDeltaTime;
                transform.localPosition = originalPosition + Random.insideUnitSphere * shakeAmount *
                    ((originalDuration - shakeDuration) / originalDuration);
                yield return new WaitForFixedUpdate();
            }
        }
    }
}