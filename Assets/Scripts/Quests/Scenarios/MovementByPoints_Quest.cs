using Assets.Scripts.Services;
using Assets.Scripts.Services.GameProgress;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class MovementByPoints_Quest : BaseQuest
    {
        [SerializeField] private MovementByPoints _movementByPoints;
        [SerializeField] private Transform _initPoint;

        private GameFactory _gameFactory;
        private TempLevelProgress _levelProgress;
        private bool _isEnd;

        [Inject]
        private void Construct(GameFactory gameFactory, ProgressService progressService)
        {
            _gameFactory = gameFactory;
            _levelProgress = progressService.TempLevelProgress;
        }

        protected override async UniTask OnRun()
        {
            await _gameFactory.CreateTransport(_initPoint.position, _initPoint.rotation);
            _movementByPoints.OnFinish += EndQuest;
            await _movementByPoints.Run();
        }

        private void EndQuest()
        {
            if (_isEnd)
                return;

            _isEnd = true;
            Debug.LogError("end");
        }
    }
}