using PickMaster.Model;
using UnityEngine;
using Zenject;

namespace PickMaster.Game.View
{
    public class RollerView : MonoBehaviour
    {
        private Roller roller;
        private Gold gold;
        
        public Roller Roller
        {
            get => roller;
        }
        
        public bool IsPositive
        {
            get => roller != null ? roller.IsPositive : true;
        }

        public void InitWithRoller(Roller roller)
        {
            this.roller = roller;
        }
        
        public class Factory : PlaceholderFactory<Roller>
        {
        
        }
    }
}