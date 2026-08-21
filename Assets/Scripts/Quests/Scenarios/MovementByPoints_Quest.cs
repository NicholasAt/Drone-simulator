using Assets.Scripts.Data.DronesData;
using Assets.Scripts.Data.HelicoptersData;
using Assets.Scripts.Data.Quests;
using Assets.Scripts.Interactive;
using Assets.Scripts.Services;
using Assets.Scripts.Services.GameProgress;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class MovementByPoints_Quest : BaseQuest
    {
        [SerializeField] private Transform _initPoint;
        [SerializeField] private Transform _movePointsRoot;
        private readonly List<Transform> _movePoints = new();

        private GameFactory _gameFactory;
        private TempLevelProgress _levelProgress;
        private int _currentPointIndex = -1;
        private bool _isEnd;

        [Inject]
        private void Construct(GameFactory gameFactory, ProgressService progressService)
        {
            _gameFactory = gameFactory;
            _levelProgress = progressService.TempLevelProgress;
        }
        private void OnValidate()
        {
            RefreshNames();
        }
        protected override async UniTask OnRun()
        {
            InitPoints();
            await CreateTransport();
            await NextPoint();
        }
        private void InitPoints()
        {
            for (int i = 0; i < _movePointsRoot.childCount; i++)
            {
                _movePoints.Add(_movePointsRoot.GetChild(i));
            }
        }
        private async UniTask CreateTransport()
        {
            switch (_levelProgress.QuestID)
            {
                case QuestID.None:
                    break;

                case QuestID.DroneMove:
                    GameObject drone = await _gameFactory.CreateDrone(DroneID.Drone1, _initPoint.position, _initPoint.rotation);
                    Camera.main.transform.SetParent(drone.transform, false);
                    break;

                case QuestID.HelicopterMove:
                    GameObject helicopter = await _gameFactory.CreateHelicopter(HelicopterID.Helicopter1, _initPoint.position, _initPoint.rotation);
                    Camera.main.transform.SetParent(helicopter.transform, false);
                    break;

                default:
                    Debug.LogError($"no logic [{_levelProgress.QuestID}]");
                    break;
            }
        }

        private async UniTask NextPoint()
        {
            _currentPointIndex++;
            if (_movePoints.Count <= _currentPointIndex)
            {
                EndQuest();
            }
            else
            {
                Transform movePoint = _movePoints[_currentPointIndex];
                GameObject questPoint = await _gameFactory.CreateQuestPoint(movePoint.position, movePoint.rotation);

                if (questPoint.TryGetComponent(out TriggerReporter triggerReporter) == false)
                    Debug.LogError("no reporter");

                triggerReporter.OnTrigger += (target) => OnTrigger(triggerReporter, target).Forget();
            }
        }
        private void EndQuest()
        {
            if (_isEnd)
                return;

            _isEnd = true;
            Debug.LogError("end");
        }
        private async UniTask OnTrigger(TriggerReporter reporter, GameObject target)
        {
            CharacterMarker character = target.GetComponentInParent<CharacterMarker>();
            if (character == false)
                return;

            reporter.OnTrigger = null;
            Destroy(reporter.gameObject);
            await NextPoint();
        }
        private void RefreshNames()
        {
            if (_movePointsRoot != null)
            {
                for (int i = 0; i < _movePointsRoot.childCount; i++)
                {
                    _movePointsRoot.GetChild(i).gameObject.name = $"Point [{i + 1}]";
                }
            }
        }
        private void OnDrawGizmos()
        {
            if (_initPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_initPoint.position, 15);
            }

            if (_movePointsRoot != null)
            {
                Gizmos.color = Color.red;
                Vector3 previousPos=Vector3.zero;
                for (int i = 0; i < _movePointsRoot.childCount; i++)
                {
                    Transform point = _movePointsRoot.GetChild(i);
                    Gizmos.DrawSphere(point.position, 5);
                    if (i != 0)
                    {
                        Gizmos.DrawLine(previousPos, point.position);
                    }
                    previousPos = point.position;
                }
            }
            RefreshNames();
        }
    }
}