using PickMaster.MainMenu;
using PickMaster.Managers;
using Zenject;

namespace PickMaster.DI
{
    public class MainSceneContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<WindowManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<EggController>().FromComponentInHierarchy().AsSingle();
        }
    }
}
