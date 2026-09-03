using Assets.Scripts.Services;
using Assets.Scripts.Services.GameProgress;
using Assets.Scripts.Services.GameStates;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class MovementByPoints_Quest : BaseQuest
    {
        [Serializable]
        private class Config
        {
            public float DieImpulse = 20;
            public int BestSeconds = 5;
            public int BadSeconds = 15;
        }
        [SerializeField] private Config _config;
        [SerializeField] private MovementByPoints _movementByPoints;
        [SerializeField] private Transform _initPoint;
        private TransportFactory _transportFactory;

        private TempLevelProgress _levelProgress;
        private UIFactory _uIFactory;
        private GameStateMachine _gameStateMachine;
        private TimerService _timerService;
        private CalculateStarsService _calculateStars;

        [Inject]
        private void Construct(TransportFactory transportFactory, ProgressService progressService, UIFactory uIFactory, GameStateMachine gameStateMachine, TimerService timerService, CalculateStarsService calculateStarsService)
        {
            _transportFactory = transportFactory;
            _levelProgress = progressService.TempLevelProgress;
            _uIFactory = uIFactory;
            _gameStateMachine = gameStateMachine;
            _timerService = timerService;
            _calculateStars = calculateStarsService;
        }

        private void OnDestroy()
        {
            if (_transportFactory.PlayerKeeper.CharacterHit != null)
            {
                _transportFactory.PlayerKeeper.CharacterHit.OnHit -= OnHit;
                _transportFactory.PlayerKeeper.CharacterHit.OnTurned -= OnTurned;
            }
        }

        protected override async UniTask OnRun()
        {
            await _transportFactory.CreateTransport(_levelProgress.QuestID, _initPoint.position, _initPoint.rotation);
            _transportFactory.PlayerKeeper.CharacterHit.OnHit += OnHit;
            _transportFactory.PlayerKeeper.CharacterHit.OnTurned += OnTurned;
            _movementByPoints.OnFinish += () => Win().Forget();
            await _movementByPoints.Run();
            _timerService.Start();
        }
        private void OnTurned()
        {
            RestartPlayer();
        }
        private async UniTask Win()
        {
            int stars = _calculateStars.Calculate(_config.BadSeconds, _config.BestSeconds, _timerService.Seconds);
            Debug.LogError($"{stars}");
            await ProtectedWin(stars);
        }
        private void OnHit(float force)
        {
            if (force > _config.DieImpulse)
                RestartPlayer();
        }

        private void RestartPlayer()
        {
            _transportFactory.PlayerKeeper.CharacterRefresher.Show(_initPoint.position, _initPoint.rotation);
        }
    }
}