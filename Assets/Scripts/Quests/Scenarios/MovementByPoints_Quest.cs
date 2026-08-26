using Assets.Scripts.Services;
using Assets.Scripts.Services.CameraService;
using Assets.Scripts.Services.GameProgress;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class MovementByPoints_Quest : BaseQuest
    {
        [SerializeField] private MovementByPoints _movementByPoints;
        [SerializeField] private Transform _initPoint;

        private GameFactory _gameFactory;
        private CameraStateService _cameraService;
        private TempLevelProgress _levelProgress;
        private bool _isEnd;
        private CancellationToken _ct;

        [Inject]
        private void Construct(GameFactory gameFactory, CameraStateService cameraService, ProgressService progressService)
        {
            _gameFactory = gameFactory;
            _cameraService = cameraService;
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
            _ct = this.GetCancellationTokenOnDestroy();
            await _gameFactory.CreateTransport(_initPoint.position, _initPoint.rotation);
            _gameFactory.PlayerKeeper.CharacterHit.OnHit += OnHit;
            _gameFactory.PlayerKeeper.CharacterHit.OnTurned += OnTurned;
            _movementByPoints.OnFinish += EndQuest;
            await _movementByPoints.Run();
        }

        private void OnTurned()
        {
            OnHit(12);
        }

        private void OnHit(float force)
        {
            if (force > 11)
            {
                Vector3 characterPos = _gameFactory.PlayerKeeper.Pos();
                _cameraService.Show(characterPos, _ct).ContinueWith(RestartPlayer);
                _gameFactory.PlayerKeeper.CharacterRefresher.Hide();
            }
        }

        private void RestartPlayer()
        {
            _gameFactory.PlayerKeeper.CharacterRefresher.Show(_initPoint.position, _initPoint.rotation);
            _cameraService.SetParent(_gameFactory.PlayerKeeper.Character.transform);
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