using Assets.Scripts.Data;
using Assets.Scripts.Data.Quests;
using Assets.Scripts.Services;
using Assets.Scripts.Services.AssetProvider;
using Assets.Scripts.Services.GameProgress;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
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
            _sceneLoader.UpdateProgress(0.4f);
            await _uIFactory.CreateMenu(CancelToken);
            _sceneLoader.UpdateProgress(0.7f);
            await _musicService.PlayBackground();
            _sceneLoader.UpdateProgress(1f);
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
            /*fast click */
            pop.Refresh("May be lags on the Safari browser", "Ok");
          
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: CancelToken);

            pop.OnButtonClick += pop.Close;
        }
    }
}