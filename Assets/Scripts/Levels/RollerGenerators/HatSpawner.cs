using DG.Tweening;
using UnityEngine;

namespace Game.View
{
    public class HatSpawner : RollerSpawner
    {
        [SerializeField] 
        private Material blackMat;
        
        [SerializeField] 
        private Material pinkMat;
        
        [SerializeField] 
        private Material goldenMat;
       
        [SerializeField] 
        private Material goldenDarkMat;

        [SerializeField] 
        private GameObject hatGO;
        
        private Sequence animHat;

        [SerializeField]
        private AudioSource birthSound;


        public override void Animate(float spawnDelay)
        {
            animHat = DOTween.Sequence()
                .Append(transform.DOShakeScale(0.3f, new Vector3(0.1f, 0.1f, 0.5f), 2, 45f, true))
                .Append(transform.DOPunchPosition(new Vector3(0, -0.1f, 0), 0.2f, 2, 1, false));
            //.Append(hat.transform.DOShakePosition(0.3f, new Vector3(0, .1f, 0), 2, 90f, false, true));
            animHat.Play();

            birthSound.pitch = 0.5f / spawnDelay;
            birthSound.Play();
        }

        public override void MakeGolden()
        {
            Material[] mats = hatGO.GetComponent<MeshRenderer>().materials;
            mats[0] = goldenMat;
            mats[1] = goldenDarkMat;

            GetComponent<MeshRenderer>().materials = mats;

            Camera.main.backgroundColor = new Color(255f / 255f, 250f / 255f, 199f / 255f, 1f);

        }

        public override void MakeNormal()
        {
            Material[] mats = hatGO.GetComponent<MeshRenderer>().materials;
            mats[0] = blackMat;
            mats[1] = pinkMat;

            GetComponent<MeshRenderer>().materials = mats;
            Camera.main.backgroundColor = new Color(251f / 255f, 223f / 255f, 255f / 255f, 1f);
        }

        public override void InitRoller(GameObject go)
        {
            
        }
    }
}