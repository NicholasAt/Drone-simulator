using Assets.Scripts.Services;
using Assets.Scripts.Services.AssetProvider;
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

            public Preparation(SceneLoader sceneLoader, IAssetProviderService assetProviderService)
            {
                _sceneLoader = sceneLoader;
                _assetProvider = assetProviderService;
            }
            public async UniTask Run()
            {
                _assetProvider.ReleaseAll();
                await _sceneLoader.LoadSingle(Constants.SceneConstants.MenuSceneKey);
            }
        }

        private UIFactory _uIFactory;
        private GameObserver _gameObserver;

        [Inject]
        private void Construct(UIFactory uIFactory, GameObserver gameObserver)
        {
            _uIFactory = uIFactory;
            _gameObserver = gameObserver;
        }

        protected override async UniTask OnStart()
        {
            _gameObserver.SendChangePause(false, false);
            await _uIFactory.CreateMenu();
        }
    }
}