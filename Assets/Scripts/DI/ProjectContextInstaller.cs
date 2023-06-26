using PickMaster.DI.Signals;
using Zenject;
using PickMaster.Managers;

namespace PickMaster.DI
{
    public class ProjectContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            #region Signals
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<GameStateChangedSignal>();
            Container.DeclareSignal<HandShownSignal>();
            Container.DeclareSignal<RollerCollectedSignal>();
            Container.DeclareSignal<IngotCollectedSignal>();
            Container.DeclareSignal<IngotSpawnedSignal>();
            Container.DeclareSignal<GoldCollectedSignal>();
            Container.DeclareSignal<LevelGoldProgressSignal>();
            Container.DeclareSignal<RollerSpawnedSignal>();
            Container.DeclareSignal<TapMadeSignal>().OptionalSubscriber();
            Container.DeclareSignal<LevelUpgradedSignal>();
            Container.DeclareSignal<SettingUpgradedSignal>();
            #endregion
            
            Container.Bind<Settings>().AsSingle();
            Container.BindInterfacesAndSelfTo<Inventory>().AsSingle();
            Container.BindInterfacesAndSelfTo<AnalyticsController>().AsSingle();
        }
    }
}