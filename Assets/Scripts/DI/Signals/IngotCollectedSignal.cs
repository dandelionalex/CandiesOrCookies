using UnityEngine;

namespace PickMaster.DI.Signals
{
    public class IngotCollectedSignal
    {
        public Vector3 CollectPosition { get; }
        
        public IngotCollectedSignal(Vector3 collectPosition)
        {
            CollectPosition = CollectPosition;
        }
    }
}