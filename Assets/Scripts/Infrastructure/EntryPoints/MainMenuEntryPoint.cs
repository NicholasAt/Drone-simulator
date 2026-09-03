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
            private readonly GameObserver _gameObserver;
            private readonly TimerService _timerService;

            public Preparation(SceneLoader sceneLoader, IAssetProviderService assetProviderService, GameObserver gameObserver,TimerService timerService)
            {
                _sceneLoader = sceneLoader;
                _assetProvider = assetProviderService;
                _gameObserver = gameObserver;
                _timerService = timerService;
            }
            public async UniTask Run()
            {
                _assetProvider.ReleaseAll();
                _timerService.Stop();
                _gameObserver.Cleanup();
                await _sceneLoader.LoadSingle(Constants.SceneConstants.MenuSceneKey);
            }
        }

        private UIFactory _uIFactory;
        private TempLevelProgress _levelProgress;

        [Inject]
        private void Construct(UIFactory uIFactory, ProgressService progressService)
        {
            _uIFactory = uIFactory;
            _levelProgress = progressService.TempLevelProgress;
        }

        protected override async UniTask OnStart()
        {
            _levelProgress.SetQuestId(QuestID.Helicopter_Move);//first quest
            await _uIFactory.CreateMenu();
        }
    }
}