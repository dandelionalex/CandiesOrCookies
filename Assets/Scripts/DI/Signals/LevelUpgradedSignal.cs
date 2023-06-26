using PickMaster.Model;

namespace PickMaster.DI.Signals
{
    public class LevelUpgradedSignal
    {
        public RollerModel RollerModel { get; private set; }

        public LevelUpgradedSignal(RollerModel rollerModel)
        {
            this.RollerModel = rollerModel;
        }
    }
}