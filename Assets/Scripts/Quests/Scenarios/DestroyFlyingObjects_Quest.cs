using Assets.Scripts.Bots;
using Assets.Scripts.Data.BotsData.FlyData;
using Assets.Scripts.Logic;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class DestroyFlyingObjects_Quest : BaseQuest<DestroyFlyingObjects_Quest.MainConfig>
    {
        [Serializable]
        private class SpawnMarker
        {
            public Transform Point;
            public float Radius;
            public FlyingTransportID Id;
        }

        [Serializable]
        public class MainConfig : BaseConfig
        {
            public int MaxTargets = 3;
            public int MinTargets = 1;

            public int Time = 15;
            public int BestKills = 5;
            public int BadKills = 15;
            [TextArea] public string WinMessage;

        }

        [SerializeField] private List<SpawnMarker> _spawnMarkers;

        [SerializeField] private WalkableArea _walkableArea;
        [SerializeField] private Transform _playerSpawnPoint;

        private TimerService _timerService;
        private CalculateStarsService _calculateStars;
        private GameFactory _gameFactory;
        private readonly List<(IApplyDamage, BotRefresher)> _targers = new();
        private int _currentKillCont;

        [Inject]
        private void Construct(GameFactory gameFactory, TimerService timerService, CalculateStarsService calculateStarsService, ShowKills showKills)
        {
            _gameFactory = gameFactory;
            _timerService = timerService;
            _calculateStars = calculateStarsService;
        }

        protected override async UniTask OnRun()
        {
            await InitTransport();

            _timerService.OnTick += OnTimerTick;
            _timerService.Start(Config.Time, false);
        }
        protected override Transform InitPoint()
        {
            return _playerSpawnPoint;
        }

        protected override void RestartPlayer()
        {
            base.RestartPlayer();
            CheckTransports();
        }

        private void OnTimerTick()
        {
            if (_timerService.Seconds <= 0)
            {
                _timerService.Stop();
                int stars = _calculateStars.InvertCalculate(Config.BadKills, Config.BestKills, _currentKillCont);
                Win(stars, $"{Config.WinMessage}: {_currentKillCont}").Forget();
            }
        }

        private async UniTask InitTransport()
        {
            _walkableArea.GetArea(out Transform matrix, out Vector3 size);
            for (int i = 0; i < base.Config.MaxTargets; i++)
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

        private void CheckTransports()
        {
            int lives = 0;
            foreach ((IApplyDamage damage, BotRefresher refresher) in _targers)
            {
                if (damage.Died == false)
                    lives++;
            }
            if (lives <= Config.MinTargets)
            {
                int difference = Config.MaxTargets - lives;

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