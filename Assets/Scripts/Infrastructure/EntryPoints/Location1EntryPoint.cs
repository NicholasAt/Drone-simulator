using Assets.Scripts.Services;
using Assets.Scripts.Services.AssetProvider;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Assets.Scripts.Infrastructure.EntryPoints
{
    public class Location1EntryPoint : BaseEntryPoint
    {
        private GameFactory _gameFactory;

        public class Preparation
        {
            private readonly SceneLoader _sceneLoader;
            private readonly IAssetProviderService _assetProvider;

            public Preparation(SceneLoader sceneLoader, IAssetProviderService assetProvider)
            {
                _sceneLoader = sceneLoader;
                _assetProvider = assetProvider;
            }
            public async UniTask Run()
            {
                _assetProvider.ReleaseAll();
                await _sceneLoader.LoadSingle(Constants.SceneConstants.Location1SceneKey);
            }
        }

        [Inject]
        private void Construct(GameFactory gameFactory)
        {
            _gameFactory = gameFactory;
        }

        protected override async UniTask OnStart()
        {
            await _gameFactory.CreateDrone();
        }
    }
}