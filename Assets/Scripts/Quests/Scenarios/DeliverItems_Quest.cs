using Assets.Scripts.Data.Quests;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class DeliverItems_Quest : BaseQuest<DeliverItems_Quest.MainConfig>
    {
        [Serializable]
        public class MainConfig : BaseConfig
        {
            [TextArea] public string WinMessage;
            public int BestSeconds = 5;
            public int BadSeconds = 15;
        }

        [SerializeField] private MovementByPoints _movementByPoints;
        [SerializeField] private Transform _initPoint;
        [SerializeField] private Transform _pointsRoot;

        private GameFactory _gameFactory;
        private QuestObjectsData _questObjectsData;
        private TimerService _timerService;
        private CalculateStarsService _calculateStars;
        private CancellationToken _ct;
        [Inject]
        private void Construct(GameFactory gameFactory, QuestObjectsData questObjectsData, TimerService timerService, CalculateStarsService calculateStarsService)
        {
            _gameFactory = gameFactory;
            _questObjectsData = questObjectsData;
            _timerService = timerService;
            _calculateStars = calculateStarsService;
        }

        private void OnValidate()
        {
            RefreshNames();
        }
        private void Awake()
        {
            _ct = this.GetCancellationTokenOnDestroy();
        }
        protected override async UniTask OnRun()
        {
            _movementByPoints.OnTriggered += (point) => Triggered(point).Forget();
            _movementByPoints.OnFinish += () => Win().Forget();

            await _movementByPoints.Run();
            _timerService.Start();
        }
        protected override Transform InitPoint()
        {
            return _initPoint;
        }

        private async UniTask Win()
        {
            int stars = _calculateStars.Calculate(Config.BadSeconds, Config.BestSeconds, _timerService.Seconds);
            await Win(stars, Config.WinMessage);
        }
        private async UniTask Triggered(Transform triggerPoint)
        {
            if (triggerPoint.childCount == 0)
            {
                Debug.LogError("no point");
                return;
            }
            Transform point = triggerPoint.GetChild(0);
          
            GameObject instance = await _gameFactory.CreateQuestObject(_questObjectsData.DeliveryItemReference, CharacterPos(), Quaternion.identity, _ct, "");
            instance.transform.DOJump(point.position, 5, 1, 1).SetEase(Ease.Linear);
            instance.transform.DORotate(point.eulerAngles, 1);
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
            if (_initPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_initPoint.position, 5);
            }
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
                        Gizmos.DrawCube(Vector3.zero, new Vector3(3, 1, 1));
                    }
                }
                RefreshNames();
            }
        }
    }
}