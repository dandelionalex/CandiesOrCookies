namespace PickMaster.DI.Signals
{
    public class LevelGoldProgressSignal
    {
        public float Total { get; }
        public float Delta { get; }

        public LevelGoldProgressSignal( float total, float delta )
        {
            this.Total = total;
            this.Delta = delta;
        }
    }
}