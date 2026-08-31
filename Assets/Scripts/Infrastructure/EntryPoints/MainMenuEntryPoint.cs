using Assets.Scripts.Services;
using Assets.Scripts.Services.AssetProvider;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Assets.Scripts.Infrastructure.EntryPoints
{
    public class MainMenuEntryPoint : BaseEntryPoint
    {
        private UIFactory _uIFactory;

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

        [Inject]
        private void Construct(UIFactory uIFactory)
        {
            _uIFactory = uIFactory;
        }

        protected override async UniTask OnStart()
        {
            await _uIFactory.CreateMenu();
        }
    }
}