using Assets.Scripts.Services;
using Assets.Scripts.Services.GameProgress;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
        private TempLevelProgress _levelProgress;
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
            await _gameFactory.CreateTransport(_initPoint.position, _initPoint.rotation);
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
                GameObject instance = await _gameFactory.CreateDeliverItem(_gameFactory.PlayerKeeper.Pos(), Quaternion.identity);
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