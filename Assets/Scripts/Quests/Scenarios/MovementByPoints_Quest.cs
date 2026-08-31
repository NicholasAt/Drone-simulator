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
        private TransportFactory _transportFactory;

        private TempLevelProgress _levelProgress;
        private bool _isEnd;

        [Inject]
        private void Construct(TransportFactory transportFactory, ProgressService progressService)
        {
            _transportFactory = transportFactory;
            _levelProgress = progressService.TempLevelProgress;
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
            _transportFactory.PlayerKeeper.CharacterRefresher.Show(_initPoint.position, _initPoint.rotation);
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