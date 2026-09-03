using Assets.Scripts.Data.Quests;
using Assets.Scripts.Services;
using Assets.Scripts.Services.GameProgress;
using Assets.Scripts.Services.GameStates;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class DeliverItems_Quest : BaseQuest
    {
        [SerializeField, TextArea] private string _winMessage;
        [SerializeField] private MovementByPoints _movementByPoints;
        [SerializeField] private Transform _initPoint;
        [SerializeField] private Transform _pointsRoot;

        private GameFactory _gameFactory;
        private GameStateMachine _gameStateMachine;
        private UIFactory _uIFactory;
        private TransportFactory _transportFactory;
        private TempLevelProgress _levelProgress;
        private QuestObjectsData _questObjectsData;
        private TimerService _timerService;

        [Inject]
        private void Construct(GameFactory gameFactory, GameStateMachine gameStateMachine, UIFactory uIFactory, TransportFactory transportFactory, ProgressService progressService, QuestObjectsData questObjectsData, TimerService timerService)
        {
            _gameFactory = gameFactory;
            _gameStateMachine = gameStateMachine;
            _uIFactory = uIFactory;
            _transportFactory = transportFactory;
            _levelProgress = progressService.TempLevelProgress;
            _questObjectsData = questObjectsData;
            _timerService = timerService;
        }
        private void OnValidate()
        {
            RefreshNames();
        }
        protected override async UniTask OnRun()
        {
            await _transportFactory.CreateTransport(_levelProgress.QuestID, _initPoint.position, _initPoint.rotation);
            _movementByPoints.OnTriggered += (point) => Triggered(point).Forget();
            _movementByPoints.OnFinish += () => ProtectedWin(_winMessage).Forget();
            await _movementByPoints.Run();
            _timerService.Start();
        }

        private async UniTask Triggered(Transform triggerPoint)
        {
            if (_movementByPoints.IsLastPoint() == false)
            {
                if (triggerPoint.childCount == 0)
                {
                    Debug.LogError("no point");
                    return;
                }
                Transform point = triggerPoint.GetChild(0);
                GameObject instance = await _gameFactory.CreateQuestObject(_questObjectsData.DeliveryItemReference, _transportFactory.PlayerKeeper.Pos(), Quaternion.identity, this.GetCancellationTokenOnDestroy());
                instance.transform.DOJump(point.position, 5, 1, 1).SetEase(Ease.Linear);
                instance.transform.DORotate(point.eulerAngles, 1);
            }
        }
        private void RefreshNames()
        {
            if (_pointsRoot != null)
            {
                for (int i = 0; i < _pointsRoot.childCount; i++)
                {
                    if (_pointsRoot.GetChild(i).childCount > 0)
                    {
                        Transform point = _pointsRoot.GetChild(i).GetChild(0);
                        point.gameObject.name = "Delivery point";
                    }
                }
            }
        }
        private void OnDrawGizmos()
        {
            if (_pointsRoot != null)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < _pointsRoot.childCount; i++)
                {
                    if (_pointsRoot.GetChild(i).childCount > 0)
                    {
                        Transform point = _pointsRoot.GetChild(i).GetChild(0);
                        Gizmos.DrawLine(point.position, _pointsRoot.GetChild(i).position);
                        Gizmos.matrix = point.localToWorldMatrix;
                        Gizmos.DrawCube(Vector3.zero, new Vector3(4, 2, 2));
                    }
                }
                RefreshNames();
            }
        }
    }
}