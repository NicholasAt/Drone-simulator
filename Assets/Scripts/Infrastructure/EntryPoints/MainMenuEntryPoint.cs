using Assets.Scripts.Data;
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
                await _sceneLoader.ShowCurtain();
                _assetProvider.ReleaseAll();
                await _sceneLoader.LoadSingle(Constants.SceneConstants.MenuSceneKey);
            }
        }

        private UIFactory _uIFactory;
        private TempLevelProgress _levelProgress;
        private MusicService _musicService;
        private SceneLoader _sceneLoader;
        private GameData _gameData;
        private static bool _isSafariMessage;

        [Inject]
        private void Construct(UIFactory uIFactory, ProgressService progressService, MusicService musicService, SceneLoader sceneLoader, GameData gameData)
        {
            _uIFactory = uIFactory;
            _levelProgress = progressService.TempLevelProgress;
            _musicService = musicService;
            _sceneLoader = sceneLoader;
            _gameData = gameData;
        }

        protected override async UniTask OnStart()
        {
            _levelProgress.SetQuestId(QuestID.Drone_Move);//first quest
            await _uIFactory.CreateMenu((progress) => _sceneLoader.UpdateProgress(progress, 0.5f), CancelToken);
            await _musicService.PlayBackground((progress) => _sceneLoader.UpdateProgress(progress, 1f), CancelToken);
            await UniTask.NextFrame();

            if (_gameData.IsMobile() == false)
            {
                if (_isSafariMessage == false)
                {
                    _isSafariMessage = true;
                    InitSafariPop().Forget();
                }
                else if (BrowserDetector.IsSafariBrowser())
                {
                    InitSafariPop().Forget();
                }
            }

            _sceneLoader.HideCurtain().Forget();
        }
        private async UniTask InitSafariPop()
        {
            UI.Windows.Popup.PopupWarningOneButton pop = await _uIFactory.CreatePopupWarning(CancelToken);
            pop.Refresh("May be lags on the Safari browser", "Ok");
            pop.OnButtonClick += pop.Close;
        }
    }
}