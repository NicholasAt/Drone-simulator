using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class MovementByPoints_Quest : BaseQuest<MovementByPoints_Quest.MainConfig>
    {
        [Serializable]
        public class MainConfig : BaseConfig
        {
            public int BestSeconds = 5;
            public int BadSeconds = 15;
        }
        [SerializeField] private MovementByPoints _movementByPoints;
        [SerializeField] private Transform _initPoint;

        private TimerService _timerService;
        private CalculateStarsService _calculateStars;

        [Inject]
        private void Construct(TimerService timerService, CalculateStarsService calculateStarsService)
        {
            _timerService = timerService;
            _calculateStars = calculateStarsService;
        }
        protected override Transform InitPoint()
        {
            return _initPoint;
        }
        protected override async UniTask OnRun()
        {
            _movementByPoints.OnFinish += () => Win().Forget();
            await _movementByPoints.Run();
            _timerService.Start();
        }

        private async UniTask Win()
        {
            int stars = _calculateStars.Calculate(Config.BadSeconds, Config.BestSeconds, _timerService.Seconds);
            await Win(stars);
        }

        private void OnDrawGizmos()
        {
            if (_initPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_initPoint.position, 2);
            }
        }
    }
}