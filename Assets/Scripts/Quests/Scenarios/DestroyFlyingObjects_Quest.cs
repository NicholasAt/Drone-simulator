using Assets.Scripts.Bots;
using Assets.Scripts.Data.BotsData.FlyData;
using Assets.Scripts.Logic;
using Assets.Scripts.Services;
using Assets.Scripts.Services.CameraService;
using Assets.Scripts.Services.GameProgress;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class DestroyFlyingObjects_Quest : BaseQuest
    {
        [Serializable]
        private class SpawnMarker
        {
            public Transform Point;
            public float Radius;
            public FlyingTransportID Id;
        }

        [Serializable]
        private class Config
        {
            public int MaxTargets = 3;
            public int MinTargets = 1;
            public float DieImpulse = 20;
            public float PlayerDamageRadius = 5;
            public int Time = 15;
            public int BestKills = 5;
            public int BadKills = 15;
            [TextArea] public string WinMessage;
            [TextArea] public string DestroyedMessage;
        }
        [SerializeField] private Config _config;
        [SerializeField] private List<SpawnMarker> _spawnMarkers;

        [SerializeField] private WalkableArea _walkableArea;
        [SerializeField] private Transform _playerSpawnPoint;

        private CameraStateService _cameraService;
        private HitHandler _hitHandler;
        private TempLevelProgress _levelProgress;
        private TimerService _timerService;
        private CalculateStarsService _calculateStars;
        private ShowKills _showKills;
        private GameFactory _gameFactory;
        private TransportFactory _transportFactory;
        private readonly List<(IApplyDamage, BotRefresher)> _targers = new();
        private int _currentKillCont;

        [Inject]
        private void Construct(GameFactory gameFactory, TransportFactory transportFactory, ProgressService progressService, CameraStateService cameraService, HitHandler hitHandler, TimerService timerService, CalculateStarsService calculateStarsService, ShowKills showKills)
        {
            _gameFactory = gameFactory;
            _transportFactory = transportFactory;
            _cameraService = cameraService;
            _hitHandler = hitHandler;
            _levelProgress = progressService.TempLevelProgress;
            _timerService = timerService;
            _calculateStars = calculateStarsService;
            _showKills = showKills;
        }
        private void OnDestroy()
        {
            if (_transportFactory.PlayerKeeper != null)
            {
                _transportFactory.PlayerKeeper.CharacterHit.OnHit -= OnPlayerHit;
                _transportFactory.PlayerKeeper.CharacterHit.OnTurned -= OnPlayerTurned;
            }
            _timerService.OnTick -= OnTimerTick;
        }
        protected override async UniTask OnRun()
        {
            _hitHandler.Init(_config.PlayerDamageRadius);
            await _transportFactory.CreateTransport(_levelProgress.QuestID, _playerSpawnPoint.position, _playerSpawnPoint.rotation);
            await InitTransport();
            _showKills.Init(PopupMessage);

            _transportFactory.PlayerKeeper.CharacterHit.OnHit += OnPlayerHit;
            _transportFactory.PlayerKeeper.CharacterHit.OnTurned += OnPlayerTurned;
            _timerService.OnTick += OnTimerTick;
            _timerService.Start(_config.Time, false);
        }

        private void OnTimerTick()
        {
            if (_timerService.Seconds <= 0)
            {
                _timerService.Stop();
                int stars = _calculateStars.InvertCalculate(_config.BadKills, _config.BestKills, _currentKillCont);
                ProtectedWin(stars, $"{_config.WinMessage}: {_currentKillCont}").Forget();
            }
        }

        private async UniTask InitTransport()
        {
            _walkableArea.GetArea(out Transform matrix, out Vector3 size);
            for (int i = 0; i < _config.MaxTargets; i++)
            {
                SpawnMarker marker = _spawnMarkers[UnityEngine.Random.Range(0, _spawnMarkers.Count - 1)];
                Vector3 pos = RandomSpawnPos();
                Quaternion rotate = RandomRotate();
                BotMovementByArea instance = await _gameFactory.CreateFlying(marker.Id, pos, rotate, this.GetCancellationTokenOnDestroy());
                instance.SetArea(matrix, size);
                if (instance.TryGetComponent(out IApplyDamage applyDamage) == false)
                {
                    Debug.LogError("no damage");
                }
                if (instance.TryGetComponent(out BotRefresher refresher) == false)
                    Debug.LogError("no refresher");
                _targers.Add((applyDamage, refresher));
                applyDamage.Happened += IncreaseKill;
            }
        }

        private void IncreaseKill()
        {
            _currentKillCont++;
        }

        private Quaternion RandomRotate()
        {
            return Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        }

        private Vector3 RandomSpawnPos()
        {
            SpawnMarker marker = _spawnMarkers[UnityEngine.Random.Range(0, _spawnMarkers.Count)];
            Vector2 randomPos = UnityEngine.Random.insideUnitCircle * marker.Radius;
            return marker.Point.TransformPoint(new Vector3(randomPos.x, 0, randomPos.y));
        }

        private void OnPlayerTurned()
        {
            if (_hitHandler.TryDamage(CharacterPos()))
            {
                _showKills.Run(_hitHandler.Targets);
                PlayAnimation();
            }
            else
            {
                ShowPopupMessage(_config.DestroyedMessage);
                RestartPlayer();
            }
        }

        private void OnPlayerHit(float impulse)
        {
            if (_hitHandler.TryDamage(CharacterPos()))
            {
                _showKills.Run(_hitHandler.Targets);
                PlayAnimation();
            }
            else if (impulse > _config.DieImpulse)
            {
                ShowPopupMessage(_config.DestroyedMessage);
                RestartPlayer();
            }
        }

        private Vector3 CharacterPos()
        {
            return _transportFactory.PlayerKeeper.Pos();
        }

        private void PlayAnimation()
        {
            _timerService.SetPause(true);
            _transportFactory.PlayerKeeper.CharacterRefresher.Hide();
            _cameraService.Enter<CameraAnimationState, Vector3, Action, CancellationToken>(CharacterPos(), RestartPlayer, this.GetCancellationTokenOnDestroy()).Forget();
        }

        private void RestartPlayer()
        {
            _cameraService.Enter<CameraToCharacterState>().Forget();
            CheckTransports();
            _timerService.SetPause(false);
            _transportFactory.PlayerKeeper.CharacterRefresher.Show(_playerSpawnPoint.position, _playerSpawnPoint.rotation);
        }

        private void CheckTransports()
        {
            int lives = 0;
            foreach ((IApplyDamage damage, BotRefresher refresher) in _targers)
            {
                if (damage.Died == false)
                    lives++;
            }
            if (lives <= _config.MinTargets)
            {
                int difference = _config.MaxTargets - lives;

                foreach ((IApplyDamage damage, BotRefresher refresher) in _targers)
                {
                    if (damage.Died)
                    {
                        difference--;
                        refresher.Refresh();
                        refresher.RespawnPosition(RandomSpawnPos(), RandomRotate());
                    }
                    if (difference <= 0)
                        break;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (_playerSpawnPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_playerSpawnPoint.position, 2);
            }

            Gizmos.color = Color.red;
            foreach (SpawnMarker marker in _spawnMarkers)
            {
                if (marker.Point != null)
                {
                    int segments = 50;
                    Vector3 previousPos = new();
                    for (int i = 0; i <= segments; i++)
                    {
                        float rad = i * (Mathf.PI * 2 / segments);
                        Vector3 rawPos = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * marker.Radius;
                        Vector3 pos = marker.Point.TransformPoint(rawPos);
                        if (i != 0)
                        {
                            Gizmos.DrawLine(previousPos, pos);
                        }
                        previousPos = pos;
                    }
                }
            }
        }
    }
}