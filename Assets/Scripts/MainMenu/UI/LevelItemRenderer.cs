using PickMaster.Model;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.UI
{
    public class LevelItemRenderer : MonoBehaviour
    {
        [SerializeField] 
        private Image itemIcon;

        public void SetItem(RollerModel roller)
        {
            itemIcon.sprite = Resources.Load<Sprite>(roller.IconName);
        }

        public void SetAsEmpty()
        {
            itemIcon.sprite = Resources.Load<Sprite>("icon_Question");
        }
    }
}