using Assets.Scripts.Data.Quests;
using Assets.Scripts.Services;
using Assets.Scripts.Services.GameProgress;
using Assets.Scripts.Services.GameStates;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class DeliverItems_Quest : BaseQuest
    {
        [Serializable]
        private class Config
        {
            [TextArea] public string WinMessage;
            [TextArea] public string DestroyedMessage;
            public float DieImpulse = 20;
            public int BestSeconds = 5;
            public int BadSeconds = 15;
        }

        [SerializeField] private Config _config;
        [SerializeField] private MovementByPoints _movementByPoints;
        [SerializeField] private Transform _initPoint;
        [SerializeField] private Transform _pointsRoot;

        private GameFactory _gameFactory;
        private GameStateMachine _gameStateMachine;
        private TransportFactory _transportFactory;
        private TempLevelProgress _levelProgress;
        private QuestObjectsData _questObjectsData;
        private TimerService _timerService;
        private CalculateStarsService _calculateStars;

        [Inject]
        private void Construct(GameFactory gameFactory, GameStateMachine gameStateMachine, TransportFactory transportFactory, ProgressService progressService, QuestObjectsData questObjectsData, TimerService timerService, CalculateStarsService calculateStarsService)
        {
            _gameFactory = gameFactory;
            _gameStateMachine = gameStateMachine;
            _transportFactory = transportFactory;
            _levelProgress = progressService.TempLevelProgress;
            _questObjectsData = questObjectsData;
            _timerService = timerService;
            _calculateStars = calculateStarsService;
        }
        private void OnDestroy()
        {
            if (_transportFactory.PlayerKeeper != null)
            {
                _transportFactory.PlayerKeeper.CharacterHit.OnHit -= OnPlayerHit;
                _transportFactory.PlayerKeeper.CharacterHit.OnTurned -= OnPlayerTurned;
            }
        }
        private void OnValidate()
        {
            RefreshNames();
        }

        protected override async UniTask OnRun()
        {
            await _transportFactory.CreateTransport(_levelProgress.QuestID, _initPoint.position, _initPoint.rotation);
            _movementByPoints.OnTriggered += (point) => Triggered(point).Forget();
            _movementByPoints.OnFinish += () => Win().Forget();

            await _movementByPoints.Run();
            _timerService.Start();

            _transportFactory.PlayerKeeper.CharacterHit.OnHit += OnPlayerHit;
            _transportFactory.PlayerKeeper.CharacterHit.OnTurned += OnPlayerTurned;
        }

        private void OnPlayerHit(float obj)
        {
            if (obj > _config.DieImpulse)
                RestartPlayer();
        }

        private void OnPlayerTurned()
        {
            RestartPlayer();
        }
        private void RestartPlayer()
        {
            ShowPopupMessage(_config.DestroyedMessage);
            _transportFactory.PlayerKeeper.CharacterRefresher.Show(_initPoint.position, _initPoint.rotation);
        }
        private async UniTask Win()
        {
            int stars = _calculateStars.Calculate(_config.BadSeconds, _config.BestSeconds, _timerService.Seconds);
            await ProtectedWin(stars, _config.WinMessage);
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
                GameObject instance = await _gameFactory.CreateQuestObject(_questObjectsData.DeliveryItemReference, _transportFactory.PlayerKeeper.Pos(), Quaternion.identity, this.GetCancellationTokenOnDestroy(), "");
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