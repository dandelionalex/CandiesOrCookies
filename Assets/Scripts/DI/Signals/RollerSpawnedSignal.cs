using PickMaster.Model;

namespace PickMaster.DI.Signals
{
    public class RollerSpawnedSignal
    {
        public Roller Roller { get; }

        public RollerSpawnedSignal(Roller roller)
        {
            Roller = roller;
        }
    }
}