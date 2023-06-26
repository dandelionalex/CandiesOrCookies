namespace PickMaster.Model
{
    public class RollerModel
    {
        public string RollerId { get; }
        public string PrefabName { get; }
        public float LevelProgress { get; }
        public string IconName { get; }

        public RollerModel(string rollerId, string prefabName, string iconName, float levelProgress)
        {
            this.RollerId = rollerId;
            this.PrefabName = prefabName;
            this.LevelProgress = levelProgress;
            this.IconName = iconName;
        }
    }
}