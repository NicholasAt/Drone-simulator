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
        private SceneLoader _sceneLoader;

        [Inject]
        private void Construct(UIFactory uIFactory, ProgressService progressService, MusicService musicService, SceneLoader sceneLoader)
        {
            _uIFactory = uIFactory;
            _levelProgress = progressService.TempLevelProgress;
            _musicService = musicService;
            _sceneLoader = sceneLoader;
        }

        protected override async UniTask OnStart()
        {
            _levelProgress.SetQuestId(QuestID.Drone_Move);//first quest
            _sceneLoader.UpdateProgress(0.4f);
            await _musicService.PlayBackground();
            _sceneLoader.UpdateProgress(0.65f);
            await _uIFactory.CreateMenu(CancelToken);
            _sceneLoader.UpdateProgress(1f);
            _sceneLoader.HideCurtain().Forget();
        }
    }
}