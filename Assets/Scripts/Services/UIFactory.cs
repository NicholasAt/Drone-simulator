using Assets.Scripts.Data;
using Assets.Scripts.Services.AssetProvider;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Assets.Scripts.Services
{
    public class UIFactory
    {
        private readonly DiContainer _diContainer;
        private readonly UIData _uIData;
        private readonly IAssetProviderService _assetProvider;

        public UIFactory(DiContainer diContainer, UIData uIData, IAssetProviderService assetProviderService)
        {
            _diContainer = diContainer;
            _uIData = uIData;
            _assetProvider = assetProviderService;
        }

        public async UniTask CreatePopupHome()
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(_uIData.PopUpHomeReference);
            InstantiateInject(prefab);
        }

        public async UniTask CreateHUD()
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(_uIData.HUDReference);
            InstantiateInject(prefab);
        }
        public async UniTask CreateMenu()
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(_uIData.MainMenuWindowReference);
            InstantiateInject(prefab);
        }
        private GameObject InstantiateInject(GameObject prefab, Transform parent = null)
        {
            GameObject instance = _diContainer.InstantiatePrefab(prefab);
            instance.transform.SetParent(parent);

            if (parent == null)
                SceneManager.MoveGameObjectToScene(instance, SceneManager.GetActiveScene());

            return instance;
        }
    }
}