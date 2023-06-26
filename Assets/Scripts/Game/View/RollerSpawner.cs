using UnityEngine;

namespace Game.View
{
    public abstract class RollerSpawner : MonoBehaviour
    {
        public abstract void Animate(float SpawnDelay);

        public abstract void MakeGolden();

        public abstract void MakeNormal();

        public abstract void InitRoller(GameObject go);
    }
}