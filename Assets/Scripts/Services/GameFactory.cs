using Assets.Scripts.Bots;
using Assets.Scripts.Data.BotsData.CarData;
using Assets.Scripts.Data.BotsData.FlyData;
using Assets.Scripts.Data.DestroyVehiclesEffect;
using Assets.Scripts.Data.DronesData;
using Assets.Scripts.Data.HelicoptersData;
using Assets.Scripts.Data.Quests;
using Assets.Scripts.Effects.Vehicles;
using Assets.Scripts.ObjecstName;
using Assets.Scripts.Quests.Scenarios;
using Assets.Scripts.Services.AssetProvider;
using Assets.Scripts.Services.GameProgress;
using Cysharp.Threading.Tasks;
using System.Threading;
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
        private readonly CarData _carData;
        private readonly FlyingTransportData _flyingTransportData;
        private readonly HelicopterData _helicopterData;

        private readonly QuestsData _questsData;
        private readonly QuestObjectsData _questObjectsData;
        private readonly ProgressService _progressService;
        private readonly VehiclesDestroyEffectData _destroyVehiclesEffectData;

        public GameFactory(DiContainer diContainer, IAssetProviderService assetProvider, DroneData droneData, CarData carData, FlyingTransportData flyingTransportData, HelicopterData helicopterData, QuestsData questsData, QuestObjectsData questObjectsData, ProgressService progressService, VehiclesDestroyEffectData destroyVehiclesEffectData)
        {
            _diContainer = diContainer;
            _assetProvider = assetProvider;
            _droneData = droneData;
            _carData = carData;
            _flyingTransportData = flyingTransportData;
            _helicopterData = helicopterData;
            _questsData = questsData;
            _questObjectsData = questObjectsData;
            _progressService = progressService;
            _destroyVehiclesEffectData = destroyVehiclesEffectData;
        }

        public async UniTask<IVehiclesDestroyEffect> CreateVehiclesDestroyEffect(DestroyEffectId id, CancellationToken ct)
        {
            VehiclesDestroyEffectConfig config = _destroyVehiclesEffectData.GetConfig(id);
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(config.Reference, ct);
            GameObject instance = InstantiateInject(prefab);
            if (instance.TryGetComponent(out IVehiclesDestroyEffect effect) == false)
                Debug.LogError("no component");

            effect.Init(id);
            return effect;
        }

        public async UniTask<GameObject> CreateQuestObject(AssetReferenceGameObject reference, Vector3 pos, Quaternion rotate, CancellationToken ct, string objectName)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(reference, ct);
            GameObject instance = InstantiateInject(prefab, pos, rotate);
            if (instance.TryGetComponent(out VehiclesDestroyEffectPlayer effectPlayer))
            {
                effectPlayer.Init(DestroyEffectId.Helicopter1);
            }
            if (instance.TryGetComponent(out IObjectName name))
            {
                name.SetName(objectName);
            }
            return instance;
        }

        public async UniTask<GameObject> CreateQuestPoint(Vector3 pos, Quaternion rotate)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(_questObjectsData.QuestPointReference);
            GameObject instance = InstantiateInject(prefab);
            instance.transform.SetPositionAndRotation(pos, rotate);
            return instance;
        }

        public async UniTask<IScenario> CreateQuest(QuestID questID)
        {
            QuestConfig cfg = _questsData.GetQuest(questID);
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(cfg.QuestReference);
            GameObject instance = InstantiateInject(prefab);
            return instance.GetComponent<IScenario>();
        }

        public async UniTask<GameObject> CreateHelicopter(HelicopterID id, Vector3 pos, Quaternion rotate)
        {
            AssetReferenceGameObject reference = _helicopterData.GetConfig(id).HelicopterReference;
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(reference);
            return InstantiateInject(prefab, pos, rotate);
        }

        public async UniTask<GameObject> CreateDrone(DroneID droneID, Vector3 pos, Quaternion rotate)
        {
            AssetReferenceGameObject reference = _droneData.GetConfig(droneID).DroneReference;
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(reference);
            return InstantiateInject(prefab, pos, rotate);
        }
        public async UniTask<BotMovementByArea> CreateFlying(FlyingTransportID id, Vector3 pos, Quaternion rotate, CancellationToken ct = default)
        {
            FlyingTransportConfig cfg = _flyingTransportData.GetConfig(id);
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(cfg.PrefabReference, ct);
            GameObject instance = InstantiateInject(prefab, pos, rotate);
            if (instance.TryGetComponent(out VehiclesDestroyEffectPlayer effectPlayer))
            {
                effectPlayer.Init(cfg.EffectId);
            }
            if (instance.TryGetComponent(out IObjectName objectName))
            {
                objectName.SetName(cfg.TransportName);
            }
            if (instance.TryGetComponent(out BotMovementByArea botFly))
            {
                botFly.Init(cfg.Speed);
                return botFly;
            }
            Debug.LogError("no component");
            return null;
        }

        public async UniTask<BotCarMove> CreateCar(CarID id, Vector3 pos, Quaternion rotate, CancellationToken ct = default)
        {
            CarConfig cfg = _carData.GetConfig(id);
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(cfg.PrefabReference, ct);
            GameObject instance = InstantiateInject(prefab, pos, rotate);

            if (instance.TryGetComponent(out VehiclesDestroyEffectPlayer effectPlayer))
            {
                effectPlayer.Init(cfg.DestroyEffectId);
            }

            if (instance.TryGetComponent(out IObjectName objectName))
            {
                objectName.SetName(cfg.Name);
            }
            if (instance.TryGetComponent(out BotCarMove botCar))
            {
                botCar.Init(cfg.Speed);
                return botCar;
            }
            Debug.LogError("no component");
            return null;
        }


        private GameObject InstantiateInject(GameObject prefab, Vector3 pos, Quaternion rotate, Transform parent = null)
        {
            GameObject instance = InstantiateInject(prefab, parent);
            if (instance.TryGetComponent(out Rigidbody rb))
            {
                instance.transform.SetPositionAndRotation(pos, rotate);
                rb.MovePosition(pos);
                rb.MoveRotation(rotate);
            }
            else
            {
                instance.transform.SetPositionAndRotation(pos, rotate);
            }
            return instance;
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