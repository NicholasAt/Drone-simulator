using Assets.Scripts.Data.Quests;
using Assets.Scripts.Services;
using Assets.Scripts.Services.GameProgress;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class DeliverItems_Quest : BaseQuest
    {
        [SerializeField] private MovementByPoints _movementByPoints;
        [SerializeField] private Transform _initPoint;
        [SerializeField] private Transform _pointsRoot;
        private GameFactory _gameFactory;
        private TransportFactory _transportFactory;
        private TempLevelProgress _levelProgress;
        private QuestObjectsData _questObjectsData;
        private bool _isEnd;
        private CancellationToken _ct;

        [Inject]
        private void Construct(GameFactory gameFactory,TransportFactory transportFactory, ProgressService progressService, QuestObjectsData questObjectsData)
        {
            _gameFactory = gameFactory;
            _transportFactory = transportFactory;
            _levelProgress = progressService.TempLevelProgress;
            _questObjectsData = questObjectsData;
        }
        private void OnValidate()
        {
            RefreshNames();
        }
        protected override async UniTask OnRun()
        {
            _ct = this.GetCancellationTokenOnDestroy();
            await _transportFactory.CreateTransport(_levelProgress.QuestID, _initPoint.position, _initPoint.rotation);
            _movementByPoints.OnTriggered += (point) => Triggered(point).Forget();
            _movementByPoints.OnFinish += EndQuest;
            await _movementByPoints.Run();
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
                GameObject instance = await _gameFactory.CreateQuestObject(_questObjectsData.DeliveryItemReference, _transportFactory.PlayerKeeper.Pos(), Quaternion.identity, _ct);
                instance.transform.DOJump(point.position, 5, 1, 1).SetEase(Ease.Linear);
                instance.transform.DORotate(point.eulerAngles, 1);
            }
        }

        private void EndQuest()
        {
            if (_isEnd)
                return;

            _isEnd = true;
            Debug.LogError("end");
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