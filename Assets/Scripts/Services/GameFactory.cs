using Assets.Scripts.Data;
using Assets.Scripts.Services.AssetProvider;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Zenject;

namespace Assets.Scripts.Services
{
    public class GameFactory
    {
        private readonly DiContainer _diContainer;
        private readonly IAssetProviderService _assetProvider;
        private readonly DroneData _droneData;

        public GameFactory(DiContainer diContainer, IAssetProviderService assetProvider, DroneData droneData)
        {
            _diContainer = diContainer;
            _assetProvider = assetProvider;
            _droneData = droneData;
        }

        public async UniTask CreateDrone()
        {
            AssetReferenceGameObject reference = _droneData.DroneConfigs[0].DroneReference;
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(reference);
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