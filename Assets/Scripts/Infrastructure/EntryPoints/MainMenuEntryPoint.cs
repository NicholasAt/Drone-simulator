using Assets.Scripts.Data.Quests;
using Assets.Scripts.Services;
using Assets.Scripts.Services.AssetProvider;
using Assets.Scripts.Services.GameProgress;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Assets.Scripts.Infrastructure.EntryPoints
{
    public class MainMenuEntryPoint : BaseEntryPoint
    {
        public class Preparation
        {
            private readonly SceneLoader _sceneLoader;
            private readonly IAssetProviderService _assetProvider;
            private readonly TimerService _timerService;

            public Preparation(SceneLoader sceneLoader, IAssetProviderService assetProviderService, TimerService timerService)
            {
                _sceneLoader = sceneLoader;
                _assetProvider = assetProviderService;
                _timerService = timerService;
            }
            public async UniTask Run()
            {
                _assetProvider.ReleaseAll();
                _timerService.Stop();
                await _sceneLoader.LoadSingle(Constants.SceneConstants.MenuSceneKey);
            }
        }

        private UIFactory _uIFactory;
        private TempLevelProgress _levelProgress;
        private GameObserver _gameObserver;

        [Inject]
        private void Construct(UIFactory uIFactory, ProgressService progressService, GameObserver gameObserver)
        {
            _uIFactory = uIFactory;
            _levelProgress = progressService.TempLevelProgress;
            _gameObserver = gameObserver;
        }

        protected override async UniTask OnStart()
        {
            _gameObserver.SendChangePause(false, false);
            _levelProgress.SetQuestId(QuestID.Drone_Move);//first quest
            await _uIFactory.CreateMenu();
        }
    }
}