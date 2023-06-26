using UnityEngine;
using PickMaster.Model;

namespace PickMaster.DI.Signals
{
    public class RollerCollectedSignal
    {
        public Roller Roller { get; }
        public Vector3 CollectPosition { get; }

        public RollerCollectedSignal(Roller roller, Vector3 collectPosition)
        {
            Roller = roller;
            CollectPosition = collectPosition;
        }
    }
}