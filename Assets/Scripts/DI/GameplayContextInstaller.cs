using PickMaster.Game.Managers;
using PickMaster.Game.UI;
using PickMaster.Game.View;
using PickMaster.Logic;
using PickMaster.Model;
using Zenject;

namespace PickMaster.DI
{
    public class GameplayContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameLogic>().AsSingle();
            Container.Bind<ConveyorBelt>().FromComponentInHierarchy().AsSingle();
            Container.Bind<RollerController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<FinishWindow>().FromComponentInHierarchy().AsSingle();
            
            //Container.BindFactory<Roller, RollerView.Factory>()>().PrefabProvider()
        }
    }
}