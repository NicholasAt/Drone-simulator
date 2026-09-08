using Assets.Scripts.Bots;
using Assets.Scripts.Character;
using Assets.Scripts.Data.DronesData;
using Assets.Scripts.Data.HelicoptersData;
using Assets.Scripts.Data.Quests;
using Assets.Scripts.Effects.Vehicles;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class TransportFactory
    {
        private readonly GameFactory _gameFactory;
        private readonly QuestsData _questsData;
        public CharacterComponentsKeeper PlayerKeeper { get; private set; }

        public TransportFactory(GameFactory gameFactory, QuestsData questsData)
        {
            _gameFactory = gameFactory;
            _questsData = questsData;
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
            Camera.main.transform.SetParent(helicopter.transform);
            Camera.main.transform.localPosition = new Vector3(0, 2, -14);//temp position
            InitTransport(helicopter);
        }
        private async UniTask CreateDrone(DroneID id, Vector3 pos, Quaternion rotate)
        {
            GameObject drone = await _gameFactory.CreateDrone(id, pos, rotate);
            Camera.main.transform.SetParent(drone.transform, false);
            Camera.main.transform.localPosition = new Vector3(0, 0.5f, -0.1f);
            InitTransport(drone);
        }
        private void InitTransport(GameObject instance)
        {
            if (instance != null)
            {
                instance.TryGetComponent(out CharacterHit hit);
                instance.TryGetComponent(out IRefreshPositions refresher);
                instance.TryGetComponent(out IVehiclesDestroyEffectPlayer destroyEffectPlayer);
                PlayerKeeper = new(instance, refresher, hit, destroyEffectPlayer);
            }
        }
    }
}