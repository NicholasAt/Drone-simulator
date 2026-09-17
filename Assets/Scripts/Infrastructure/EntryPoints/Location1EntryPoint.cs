using Assets.Scripts.Data;
using Assets.Scripts.Quests.Scenarios;
using Assets.Scripts.Services;
using Assets.Scripts.Services.AssetProvider;
using Assets.Scripts.Services.CameraService;
using Assets.Scripts.Services.GameProgress;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Infrastructure.EntryPoints
{
    public class Location1EntryPoint : BaseEntryPoint
    {
        public class Preparation
        {
            private readonly SceneLoader _sceneLoader;
            private readonly IAssetProviderService _assetProvider;
            private readonly CleanupService _cleanupService;

            public Preparation(SceneLoader sceneLoader, IAssetProviderService assetProvider, CleanupService cleanupService)
            {
                _sceneLoader = sceneLoader;
                _assetProvider = assetProvider;
                _cleanupService = cleanupService;
            }
            public async UniTask Run()
            {
                _cleanupService.Cleanup();
                await _sceneLoader.ShowCurtain();
                _assetProvider.ReleaseAll();
                await _sceneLoader.LoadSingle(Constants.SceneConstants.Location1SceneKey);
            }
        }

        private GameFactory _gameFactory;
        private UIFactory _uIFactory;
        private TempLevelProgress _levelProgress;
        private CameraStateService _cameraService;
        private CleanupService _cleanupService;
        private SceneLoader _sceneLoader;
        private GameData _gameData;

        [Inject]
        private void Construct(GameFactory gameFactory, UIFactory uIFactory, ProgressService progressService, CameraStateService cameraService, CleanupService cleanupService, SceneLoader sceneLoader, GameData gameData)
        {
            _gameFactory = gameFactory;
            _uIFactory = uIFactory;
            _levelProgress = progressService.TempLevelProgress;
            _cameraService = cameraService;
            _cleanupService = cleanupService;
            _sceneLoader = sceneLoader;
            _gameData = gameData;
        }

        protected override async UniTask OnStart()
        {
            _cleanupService.Cleanup();
            _sceneLoader.UpdateProgress(0.1f);
            await _gameFactory.CreateCinemaCamera(CancelToken);
            _sceneLoader.UpdateProgress(0.2f);
            await _cameraService.Prepare();
            _sceneLoader.UpdateProgress(0.3f);
            await _gameFactory.CreateBases(CancelToken);
            _sceneLoader.UpdateProgress(0.5f);

            _gameFactory.CreateOutsideMap(CancelToken).Forget();
            _uIFactory.CreateHUD(CancelToken).Forget();
            _uIFactory.CreateScreenTarget(CancelToken).Forget();
            _gameFactory.CreateClouds(CancelToken).Forget();

            IScenario qeustInstance = await _gameFactory.CreateQuest(_levelProgress.QuestID, CancelToken);
            await qeustInstance.Run();
            _sceneLoader.UpdateProgress(1f);

            if (_gameData.IsMobile())
                await _uIFactory.CreateMobileInput(CancelToken);

            _sceneLoader.HideCurtain().Forget();
        }
    }
}