using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.View
{
    public class AztecHeadSpawner : RollerSpawner
    {

        [SerializeField]
        private float birthPower;

        [SerializeField]
        private GameObject objGenerator;

        [SerializeField]
        private GameObject sparksFX;

        [SerializeField]
        private AudioSource birthSound;

        public override void Animate(float spawnDelay)
        {
            print("Anim playing");
            DOTween.Restart("HeadAnim");
            GameObject newBirthFX = Instantiate(sparksFX, objGenerator.transform);
            Destroy(newBirthFX, 2);

            birthSound.pitch = 0.5f / spawnDelay;
            birthSound.Play();

        }

        public override void InitRoller(GameObject go)
        {
            Rigidbody rb = go.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.forward * birthPower * -1, ForceMode.Impulse);
            rb.AddTorque(Vector3.right * Random.Range(0, 20f), ForceMode.Impulse);
        }

        public override void MakeGolden()
        {
            Camera.main.backgroundColor = new Color(70f / 255f, 56f / 255f, 39f / 255f, 1f);
        }

        public override void MakeNormal()
        {
            Camera.main.backgroundColor = new Color(94f / 255f, 123f / 255f, 101f / 255f, 1f);
        }
    }
}


