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
            private readonly TimerService _timerService;

            public Preparation(SceneLoader sceneLoader, IAssetProviderService assetProvider, TimerService timerService)
            {
                _sceneLoader = sceneLoader;
                _assetProvider = assetProvider;
                _timerService = timerService;
            }
            public async UniTask Run()
            {
                _assetProvider.ReleaseAll();
                _timerService.Stop();
                await _sceneLoader.LoadSingle(Constants.SceneConstants.Location1SceneKey);
            }
        }

        private GameFactory _gameFactory;
        private GameObserver _gameObserver;
        private UIFactory _uIFactory;
        private TempLevelProgress _levelProgress;
        private CameraStateService _cameraService;

        [Inject]
        private void Construct(GameFactory gameFactory, GameObserver gameObserver, UIFactory uIFactory, ProgressService progressService, CameraStateService cameraService)
        {
            _gameFactory = gameFactory;
            _gameObserver = gameObserver;
            _uIFactory = uIFactory;
            _levelProgress = progressService.TempLevelProgress;
            _cameraService = cameraService;
        }

        protected override async UniTask OnStart()
        {
            _gameObserver.SendChangePause(false, false);
            _cameraService.Prepare();
            await _uIFactory.CreateHUD();
            Quests.Scenarios.BaseQuest qeustInstance = await _gameFactory.CreateQuest(_levelProgress.QuestID);
            await qeustInstance.Run();
        }
    }
}