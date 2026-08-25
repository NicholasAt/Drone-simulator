using Assets.Scripts.Data.DronesData;
using Assets.Scripts.Data.HelicoptersData;
using Assets.Scripts.Data.Quests;
using Assets.Scripts.Quests;
using Assets.Scripts.Quests.Scenarios;
using Assets.Scripts.Services.AssetProvider;
using Assets.Scripts.Services.GameProgress;
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
        private readonly HelicopterData _helicopterData;

        private readonly QuestsData _questsData;
        private readonly QuestObjectsData _questObjectsData;
        private readonly ProgressService _progressService;
        public GameObject Player { get; private set; }

        public GameFactory(DiContainer diContainer, IAssetProviderService assetProvider, DroneData droneData, HelicopterData helicopterData, QuestsData questsData, QuestObjectsData questObjectsData, ProgressService progressService)
        {
            _diContainer = diContainer;
            _assetProvider = assetProvider;
            _droneData = droneData;
            _helicopterData = helicopterData;
            _questsData = questsData;
            _questObjectsData = questObjectsData;
            _progressService = progressService;
        }

        public async UniTask<GameObject> CreateDeliverItem(Vector3 pos, Quaternion rotate)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(_questObjectsData.DeliveryItemReference);
            GameObject instance = InstantiateInject(prefab, pos, rotate);
            return instance;
        }

        public async UniTask<GameObject> CreateQuestPoint(Vector3 pos, Quaternion rotate)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(_questObjectsData.QuestPointReference);
            GameObject instance = InstantiateInject(prefab);
            instance.transform.SetPositionAndRotation(pos, rotate);
            return instance;
        }

        public IQuest CreateQuest(QuestID questID)
        {
            QuestConfig cfg = _questsData.GetQuest(questID);
            IQuest questTemplate = cfg.QuestRunnerPrefab.Value;

            if (questTemplate is BaseQuest baseQuest)
            {
                GameObject instance = InstantiateInject(baseQuest.gameObject);
                return instance.GetComponent<IQuest>();
            }

            Debug.LogError("no logic");
            return null;
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
        public async UniTask CreateTransport(Vector3 pos, Quaternion rotate)
        {
            switch (_progressService.TempLevelProgress.QuestID)
            {
                case QuestID.DroneMove:
                    GameObject drone = await CreateDrone(DroneID.Drone1, pos, rotate);
                    Camera.main.transform.SetParent(drone.transform, false);
                    Player = drone;
                    break;

                case QuestID.HelicopterMove:
                    GameObject helicopterMove = await CreateHelicopter(HelicopterID.Helicopter1, pos, rotate);
                    Camera.main.transform.SetParent(helicopterMove.transform);
                    Camera.main.transform.localPosition = new Vector3(0, 2, -14);
                    Player = helicopterMove;
                    break;

                case QuestID.HelicopterDelivery:
                    GameObject helicopterDelivery = await CreateHelicopter(HelicopterID.Helicopter1, pos, rotate);
                    Camera.main.transform.SetParent(helicopterDelivery.transform);
                    Camera.main.transform.localPosition = new Vector3(0, 2, -14);
                    Player = helicopterDelivery;
                    break;

                case QuestID.None:
                default:
                    Debug.LogError($"no logic [{_progressService.TempLevelProgress.QuestID}]");
                    break;
            }
        }
        private GameObject InstantiateInject(GameObject prefab, Vector3 pos, Quaternion rotate, Transform parent = null)
        {
            GameObject instance = InstantiateInject(prefab, parent);
            if (instance.TryGetComponent(out Rigidbody rb))
            {
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