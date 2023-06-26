using UnityEngine;

namespace PickMaster.DI.Signals
{
    public class GoldCollectedSignal
    {
        public int GoldAmount { get; }
        
        public GoldCollectedSignal(int goldAmount )
        {
            GoldAmount = goldAmount;
        }
    }
}