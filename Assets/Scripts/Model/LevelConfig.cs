namespace PickMaster.Model
{
    /// <summary>
    /// LevelConfig depends on open rollers amount
    /// </summary>
    public class LevelConfig
    {
        public int Id = 0;
        public int Duration = 10;
        public int GoldenRainDuration = 10;
        public float RollChance = 0.32f;
        public int NextUpgradePrice = 100;
        public float RollerCoefficient = 1;
        public float SpawnDelay = 0.5f;
        public float SpawnDelayDecrease = 0.1f;
        public float SpawnDelayDecreaseTimeout = 10;
        public float SpawnDelayMinimum = 0.3f;
        public float GoldBonusModifier = 1;
    }
}