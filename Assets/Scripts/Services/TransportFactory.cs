using Assets.Scripts.Bots;
using Assets.Scripts.Character;
using Assets.Scripts.Data.DronesData;
using Assets.Scripts.Data.HelicoptersData;
using Assets.Scripts.Data.Quests;
using Assets.Scripts.Effects.Vehicles;
using Assets.Scripts.Services.CameraService;
using Assets.Scripts.VehicleCamera;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class TransportFactory
    {
        private readonly GameFactory _gameFactory;
        private readonly CharacterComponentsKeeperService _componentsKeeper;
        private readonly QuestsData _questsData;
        private readonly CameraStateService _cameraState;

        public TransportFactory(GameFactory gameFactory, CharacterComponentsKeeperService characterComponentsKeeperService, QuestsData questsData, CameraStateService cameraStateService)
        {
            _gameFactory = gameFactory;
            _componentsKeeper = characterComponentsKeeperService;
            _questsData = questsData;
            _cameraState = cameraStateService;
        }

        public async UniTask CreateTransport(QuestID questID, Vector3 pos, Quaternion rotate)
        {
            switch (questID)
            {
                case QuestID.Helicopter_Move:
                case QuestID.Helicopter_Delivery:
                    await CreateHelicopter(HelicopterID.Helicopter1, pos, rotate);
                    break;

                case QuestID.Drone_Move:
                case QuestID.Drone_DestroyMovingCar:
                case QuestID.Drone_DestroyFlyingObjects:
                case QuestID.Drone_DestroyStatic:
                    await CreateDrone(DroneID.Drone1, pos, rotate);
                    break;

                case QuestID.None:
                default:
                    Debug.LogError($"no logic [{questID}]");
                    break;
            }
        }

        private async UniTask CreateHelicopter(HelicopterID id, Vector3 pos, Quaternion rotate)
        {
            GameObject helicopter = await _gameFactory.CreateHelicopter(id, pos, rotate);
            InitTransport(helicopter);
            await _cameraState.Enter<CameraToCharacterState>();
        }
        private async UniTask CreateDrone(DroneID id, Vector3 pos, Quaternion rotate)
        {
            GameObject drone = await _gameFactory.CreateDrone(id, pos, rotate);
            InitTransport(drone);
            await _cameraState.Enter<CameraToCharacterState>();
        }
        private void InitTransport(GameObject instance)
        {
            if (instance != null)
            {
                instance.TryGetComponent(out CharacterHit hit);
                instance.TryGetComponent(out IRefreshPositions refresher);
                instance.TryGetComponent(out IVehiclesDestroyEffectPlayer destroyEffectPlayer);
                instance.TryGetComponent(out IVehicleCamera vehicleCamera);
                _componentsKeeper.Init(instance, refresher, hit, destroyEffectPlayer, vehicleCamera);
            }
        }
    }
}