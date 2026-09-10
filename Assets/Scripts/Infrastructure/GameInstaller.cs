using Assets.Scripts.Character;
using Assets.Scripts.Data;
using Assets.Scripts.Infrastructure.EntryPoints;
using Assets.Scripts.Logic;
using Assets.Scripts.Pool;
using Assets.Scripts.Services;
using Assets.Scripts.Services.AssetProvider;
using Assets.Scripts.Services.CameraService;
using Assets.Scripts.Services.ChunkLoad;
using Assets.Scripts.Services.GameProgress;
using Assets.Scripts.Services.GameStates;
using Assets.Scripts.Services.InputService;
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

            Container.Bind<StreamingChunk>().AsTransient();
            Container.Bind<HitHandler>().AsTransient();
            Container.Bind<ShowKills>().AsTransient();

            Container.Bind<CharacterComponentsKeeperService>().AsSingle();
            Container.Bind<CalculateStarsService>().AsSingle();
            Container.Bind<VehiclesDestroyPool>().AsSingle();
            Container.Bind<ChunkLoaderService>().AsSingle();
            Container.Bind<CameraStateService>().AsSingle();
            Container.Bind<OffScreenContainer>().AsSingle();
            Container.Bind<AudioMixerService>().AsSingle();
            Container.Bind<TransportFactory>().AsSingle();
            Container.Bind<ProgressService>().AsSingle();
            Container.Bind<CleanupService>().AsSingle();
            Container.Bind<GameObserver>().AsSingle();
            Container.Bind<TimerService>().AsSingle();
            Container.Bind<GameFactory>().AsSingle();
            Container.Bind<SceneLoader>().AsSingle();
            Container.Bind<PauseState>().AsSingle();
            Container.Bind<UIFactory>().AsSingle();

            Container.BindInterfacesAndSelfTo<AddressablesLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputService>().AsSingle();

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
            Container.BindInstance(data.CarData).AsSingle();
            Container.BindInstance(data.CameraData).AsSingle();
            Container.BindInstance(data.FlyingTransportData).AsSingle();
            Container.BindInstance(data.OutScreenData).AsSingle();
            Container.BindInstance(data.DestroyVehiclesEffectData).AsSingle();
            Container.BindInstance(data.ChunkData).AsSingle();
            Container.BindInstance(data.GameData).AsSingle();
        }

        private void BindStates()
        {
            Container.Bind<MainMenuEntryPoint.Preparation>().AsSingle();
            Container.Bind<Location1EntryPoint.Preparation>().AsSingle();
            Container.Bind<GameStateMachine>().AsSingle();
        }
    }
}
