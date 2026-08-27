using Assets.Scripts.Bots;
using Assets.Scripts.Data.BotsData.FlyData;
using Assets.Scripts.Logic;
using Assets.Scripts.Services;
using Assets.Scripts.Services.CameraService;
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
        public class SpawnMarker
        {
            public Transform Point;
            public float Radius;
            public FlyingTransportID Id;
        }

        [Serializable]
        public class Config
        {
            public int MaxTargets = 3;
            public int MinTargets = 1;
            public float DieImpulse = 15;
        }
        [SerializeField] private Config _config;

        [SerializeField] private WalkableArea _walkableArea;
        [SerializeField] private float _playerDamageRadius = 5;
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private List<SpawnMarker> _spawnMarkers;

        private CameraStateService _cameraService;
        private HitHandler _hitHandler;
        private GameFactory _gameFactory;
        private CancellationToken _ct;
        private bool _isEnd;
        private readonly List<(IApplyDamage, BotRefresher)> _targers = new();

        [Inject]
        private void Construct(GameFactory gameFactory, CameraStateService cameraService, HitHandler hitHandler)
        {
            _gameFactory = gameFactory;
            _cameraService = cameraService;
            _hitHandler = hitHandler;
        }
        private void OnDestroy()
        {
            if (_gameFactory.PlayerKeeper != null)
            {
            _gameFactory.PlayerKeeper.CharacterHit.OnHit -= OnPlayerHit;
            _gameFactory.PlayerKeeper.CharacterHit.OnTurned -= OnPlayerTurned;
            }
        }
        protected override async UniTask OnRun()
        {
            _ct = this.GetCancellationTokenOnDestroy();
            _hitHandler.Init(_playerDamageRadius);
            await _gameFactory.CreateTransport(_playerSpawnPoint.position, _playerSpawnPoint.rotation);
            await InitTransport();

            _gameFactory.PlayerKeeper.CharacterHit.OnHit += OnPlayerHit;
            _gameFactory.PlayerKeeper.CharacterHit.OnTurned += OnPlayerTurned;
        }

        private async UniTask InitTransport()
        {
            _walkableArea.GetArea(out Transform matrix, out Vector3 size);
            for (int i = 0; i < _config.MaxTargets; i++)
            {
                SpawnMarker marker = _spawnMarkers[UnityEngine.Random.Range(0, _spawnMarkers.Count - 1)];
                Vector3 pos = RandomSpawnPos();
                Quaternion rotate = RandomRotate();
                BotMovementByArea instance = await _gameFactory.CreateFlying(marker.Id, pos, rotate, _ct);
                instance.SetArea(matrix, size);
                if (instance.TryGetComponent(out IApplyDamage applyDamage) == false)
                {
                    Debug.LogError("no damage");
                }
                if (instance.TryGetComponent(out BotRefresher refresher) == false)
                    Debug.LogError("no refresher");
                _targers.Add((applyDamage, refresher));
            }
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
                PlayAnimation();
            }
            else
                RestartPlayer();
        }

        private void OnPlayerHit(float impulse)
        {
            if (_hitHandler.TryDamage(CharacterPos()))
            {
                PlayAnimation();
            }
            else if (impulse > _config.DieImpulse)
                RestartPlayer();
        }

        private Vector3 CharacterPos()
        {
            return _gameFactory.PlayerKeeper.Pos();
        }

        private void PlayAnimation()
        {
            _cameraService.Show(CharacterPos(), _ct).ContinueWith(RestartPlayer);
            _gameFactory.PlayerKeeper.CharacterRefresher.Hide();
        }

        private void RestartPlayer()
        {
            _gameFactory.PlayerKeeper.CharacterRefresher.Show(_playerSpawnPoint.position, _playerSpawnPoint.rotation);
            _cameraService.SetParent(_gameFactory.PlayerKeeper.Character.transform);
            CheckTransports();
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