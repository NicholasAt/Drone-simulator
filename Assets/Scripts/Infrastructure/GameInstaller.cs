using Assets.Scripts.Data;
using Assets.Scripts.Infrastructure.EntryPoints;
using Assets.Scripts.Services;
using Assets.Scripts.Services.GameStates;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Assets.Scripts.Infrastructure
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindData();
            BindStates();

            Container.Bind<SceneLoader>().AsSingle();
            Container.Bind<UIFactory>().AsSingle();

            if (CheckingForUpdates.DataContainerHandle.IsValid())
                Addressables.Release(CheckingForUpdates.DataContainerHandle);
        }

        private void BindData()
        {
            AllDataContainer data = CheckingForUpdates.DataContainer;
            Container.BindInstance(data.DroneData).AsSingle();
            Container.BindInstance(data.UIData).AsSingle();
        }

        private void BindStates()
        {
            Container.Bind<GameStateMachine>().AsSingle();
            Container.Bind<MainMenuEntryPoint.Preparation>().AsSingle();
        }
    }
}
