using Assets.Scripts.Data;
using Assets.Scripts.Quests.Scenarios;
using Assets.Scripts.Services;
using Assets.Scripts.Services.AssetProvider;
using Assets.Scripts.Services.CameraService;
using Assets.Scripts.Services.GameProgress;
using Cysharp.Threading.Tasks;
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
                await _sceneLoader.ShowCurtain();
                _cleanupService.Cleanup();
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
            await _gameFactory.CreateCinemaCamera((progress) => _sceneLoader.UpdateProgress(progress, 0.2f), CancelToken);
            await _cameraService.Prepare();
            await _gameFactory.CreateBases(null, CancelToken);
            _gameFactory.CreateClouds(CancelToken).Forget();

            _gameFactory.CreateOutsideMap((progress) => _sceneLoader.UpdateProgress(progress, 0.3f), CancelToken).Forget();
            _uIFactory.CreateHUD((progress) => _sceneLoader.UpdateProgress(progress, 0.5f), CancelToken).Forget();
            _uIFactory.CreateScreenTarget((progress) => _sceneLoader.UpdateProgress(progress, 0.6f), CancelToken).Forget();

            IScenario qeustInstance = await _gameFactory.CreateQuest(_levelProgress.QuestID, CancelToken);
            await qeustInstance.Run();
            _sceneLoader.UpdateProgress(1f, 1f);

            if (_gameData.IsMobile())
                await _uIFactory.CreateMobileInput(CancelToken);

            _sceneLoader.HideCurtain().Forget();
        }
    }
}