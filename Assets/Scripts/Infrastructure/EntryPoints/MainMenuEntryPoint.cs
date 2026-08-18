using Assets.Scripts.Services;
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

            public Preparation(SceneLoader sceneLoader)
            {
                _sceneLoader = sceneLoader;
            }
            public async UniTask Run()
            {
                await _sceneLoader.Load(Constants.SceneConstants.MenuSceneKey);
            }
        }

        [Inject]
        private void Construct(UIFactory uIFactory)
        {
            _uIFactory = uIFactory;
        }

        protected override async UniTask OnStart()
        {
            _uIFactory.CreateMenu();
            await UniTask.Yield();
        }
    }
}