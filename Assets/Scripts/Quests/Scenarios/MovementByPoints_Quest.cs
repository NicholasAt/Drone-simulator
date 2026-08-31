using Assets.Scripts.Services;
using Assets.Scripts.Services.GameProgress;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class MovementByPoints_Quest : BaseQuest
    {
        [SerializeField] private float _dieImpulse = 20;
        [SerializeField] private MovementByPoints _movementByPoints;
        [SerializeField] private Transform _initPoint;
        private TransportFactory _gameFactory;

        private TempLevelProgress _levelProgress;
        private bool _isEnd;

        [Inject]
        private void Construct(TransportFactory transportFactory, ProgressService progressService)
        {
            _gameFactory = transportFactory;
            _levelProgress = progressService.TempLevelProgress;
        }

        private void OnDestroy()
        {
            if (_gameFactory.PlayerKeeper.CharacterHit != null)
            {
                _gameFactory.PlayerKeeper.CharacterHit.OnHit -= OnHit;
                _gameFactory.PlayerKeeper.CharacterHit.OnTurned -= OnTurned;
            }
        }

        protected override async UniTask OnRun()
        {
            await _gameFactory.CreateTransport(_levelProgress.QuestID, _initPoint.position, _initPoint.rotation);
            _gameFactory.PlayerKeeper.CharacterHit.OnHit += OnHit;
            _gameFactory.PlayerKeeper.CharacterHit.OnTurned += OnTurned;
            _movementByPoints.OnFinish += EndQuest;
            await _movementByPoints.Run();
        }

        private void OnTurned()
        {
            RestartPlayer();
        }

        private void OnHit(float force)
        {
            if (force > _dieImpulse)
                RestartPlayer();
        }

        private void RestartPlayer()
        {
            _gameFactory.PlayerKeeper.CharacterRefresher.Show(_initPoint.position, _initPoint.rotation);
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