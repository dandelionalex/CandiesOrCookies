using Enums;
using UnityEngine;

namespace PickMaster.Managers
{
    public class WindowManager : MonoBehaviour
    {
        [SerializeField] 
        private GameObject levelItems;
        public GameObject OpenWindow(Window window)
        {
            switch (window)
            {
                case Window.LevelItems:
                    levelItems.SetActive(true);
                    return levelItems;
            }

            return null;
        }
    }
}