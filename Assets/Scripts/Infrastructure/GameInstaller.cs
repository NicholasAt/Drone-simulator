using Assets.Scripts.Data;
using Assets.Scripts.Infrastructure.EntryPoints;
using Assets.Scripts.Services;
using Assets.Scripts.Services.AssetProvider;
using Assets.Scripts.Services.GameProgress;
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
            Container.Bind<GameFactory>().AsSingle();
            Container.Bind<ProgressService>().AsSingle();
            Container.BindInterfacesAndSelfTo<AddressablesLoader>().AsSingle();

            if (CheckingForUpdates.DataContainerHandle.IsValid())
                Addressables.Release(CheckingForUpdates.DataContainerHandle);
        }

        private void BindData()
        {
            AllDataContainer data = CheckingForUpdates.DataContainer;
            Container.BindInstance(data.DroneData).AsSingle();
            Container.BindInstance(data.HelicopterData).AsSingle();
            Container.BindInstance(data.UIData).AsSingle();
            Container.BindInstance(data.LocationData).AsSingle();
            Container.BindInstance(data.QuestsData).AsSingle();
            Container.BindInstance(data.QuestObjectsData).AsSingle();
        }

        private void BindStates()
        {
            Container.Bind<MainMenuEntryPoint.Preparation>().AsSingle();
            Container.Bind<Location1EntryPoint.Preparation>().AsSingle();
            Container.Bind<GameStateMachine>().AsSingle();
        }
    }
}
