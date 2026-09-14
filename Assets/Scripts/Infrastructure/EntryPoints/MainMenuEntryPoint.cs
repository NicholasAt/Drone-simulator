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
            private readonly CleanupService _cleanupService;

            public Preparation(SceneLoader sceneLoader, IAssetProviderService assetProviderService, CleanupService cleanupService)
            {
                _sceneLoader = sceneLoader;
                _assetProvider = assetProviderService;
                _cleanupService = cleanupService;
            }
            public async UniTask Run()
            {
                _cleanupService.Cleanup();
                _assetProvider.ReleaseAll();
                await _sceneLoader.LoadSingle(Constants.SceneConstants.MenuSceneKey);
            }
        }

        private UIFactory _uIFactory;
        private TempLevelProgress _levelProgress;
        private MusicService _musicService;

        [Inject]
        private void Construct(UIFactory uIFactory, ProgressService progressService, MusicService musicService)
        {
            _uIFactory = uIFactory;
            _levelProgress = progressService.TempLevelProgress;
            _musicService = musicService;
        }

        protected override async UniTask OnStart()
        {
            _levelProgress.SetQuestId(QuestID.Drone_Move);//first quest
            await _musicService.PlayBackground();
            await _uIFactory.CreateMenu(CancelToken);
        }
    }
}