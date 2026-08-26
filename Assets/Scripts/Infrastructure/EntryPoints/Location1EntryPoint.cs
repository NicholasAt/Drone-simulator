using Assets.Scripts.Quests;
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
        private GameFactory _gameFactory;
        private TempLevelProgress _levelProgress;
        private CameraStateService _cameraService;

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
        private void Construct(GameFactory gameFactory, ProgressService progressService,CameraStateService cameraService)
        {
            _gameFactory = gameFactory;
            _levelProgress = progressService.TempLevelProgress;
            _cameraService = cameraService;
        }

        protected override async UniTask OnStart()
        {
            _cameraService.Prepare();
            IQuest qeustInstance = _gameFactory.CreateQuest(_levelProgress.QuestID);
            await qeustInstance.Run();
        }
    }
}