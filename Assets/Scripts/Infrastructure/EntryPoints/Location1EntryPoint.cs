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
                _assetProvider.ReleaseAll();
                _cleanupService.Cleanup();
                await _sceneLoader.LoadSingle(Constants.SceneConstants.Location1SceneKey);
            }
        }

        private GameFactory _gameFactory;
        private UIFactory _uIFactory;
        private TempLevelProgress _levelProgress;
        private CameraStateService _cameraService;
        private CleanupService _cleanupService;

        [Inject]
        private void Construct(GameFactory gameFactory, UIFactory uIFactory, ProgressService progressService, CameraStateService cameraService, CleanupService cleanupService)
        {
            _gameFactory = gameFactory;
            _uIFactory = uIFactory;
            _levelProgress = progressService.TempLevelProgress;
            _cameraService = cameraService;
            _cleanupService = cleanupService;
        }

        protected override async UniTask OnStart()
        {
            _cleanupService.Cleanup();
            await _cameraService.Prepare();
            await _uIFactory.CreateHUD(this.GetCancellationTokenOnDestroy());
            Quests.Scenarios.BaseQuest qeustInstance = await _gameFactory.CreateQuest(_levelProgress.QuestID);
            _uIFactory.CreateScreenTarget(this.GetCancellationTokenOnDestroy()).Forget(UnityEngine.Debug.LogException);
            await qeustInstance.Run();
        }
    }
}