namespace PickMaster.DI.Signals
{
    public class SettingUpgradedSignal
    {
        public int SettingId { get; }

        public SettingUpgradedSignal(int settingsId)
        {
            SettingId = settingsId;
        }
    }
}