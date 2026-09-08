using Assets.Scripts.Bots;
using Assets.Scripts.Character;
using Assets.Scripts.Data.BotsData.CarData;
using Assets.Scripts.Logic;
using Assets.Scripts.Services;
using Assets.Scripts.Services.CameraService;
using Assets.Scripts.Services.GameProgress;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class DestroyMovingCar_Quest : BaseQuest<DestroyMovingCar_Quest.MainConfig>
    {
        [Serializable]
        private class PointsMarker
        {
            public Transform Root;
            public Transform EndPoint;
            public CarID CarID;
        }
        [Serializable]
        public class MainConfig : BaseConfig
        {
            [TextArea] public string WinMessage;
            [TextArea] public string LoseMessage;
            public int BestSeconds = 5;
            public int BadSeconds = 15;
        }
        [SerializeField] private List<PointsMarker> _pointsMarker;
        [SerializeField] private TriggerReporter _loseReporter;
        [SerializeField] private Transform _spawnPlayerPoint;
        private TimerService _timerService;
        private CalculateStarsService _calculateStars;
        private GameFactory _gameFactory;

        private int _currentCarrs;

        [Inject]
        private void Construct(GameFactory gameFactory, TimerService timerService, CalculateStarsService calculateStarsService)
        {
            _gameFactory = gameFactory;
            _timerService = timerService;
            _calculateStars = calculateStarsService;
        }
        private void OnValidate()
        {
            RefreshName();
        }
        protected override async UniTask OnRun()
        {
            await InitCars();

            _loseReporter.OnTrigger += OnLose;
            _timerService.Start();
        }
        protected override (Vector3 pos, Quaternion rotate) PositionAndRotate()
        {
            return (_spawnPlayerPoint.position, _spawnPlayerPoint.rotation);
        }

        private async UniTask InitCars()
        {
            foreach (PointsMarker marker in _pointsMarker)
            {
                for (int i = 0; i < marker.Root.childCount; i++)
                {
                    Transform point = marker.Root.GetChild(i);
                    BotCarMove car = await _gameFactory.CreateCar(marker.CarID, point.position, point.rotation, this.GetCancellationTokenOnDestroy());
                    car.SetPoint(marker.EndPoint.position);
                    if (car.TryGetComponent(out IApplyDamage applyDamage) == false)
                    {
                        Debug.LogError("no apply damage");
                    }
                    _currentCarrs++;
                    applyDamage.Happened += OnDestroyCar;
                }
            }
        }

        private void OnDestroyCar()
        {
            _currentCarrs--;
            if (_currentCarrs <= 0)
            {
                int stars = _calculateStars.Calculate(Config.BadSeconds, Config.BestSeconds, _timerService.Seconds);
                ProtectedWin(stars, Config.WinMessage).Forget(Debug.LogError);
            }
        }

        private void OnLose(GameObject obj)
        {
            CharacterMarker cahracter = obj.GetComponentInParent<CharacterMarker>();
            if (cahracter != null && cahracter.IsBot)
                ProtectedLose(0, Config.LoseMessage).Forget(Debug.LogError);
        }

        private void RefreshName()
        {
            if (_pointsMarker != null)
            {
                foreach (PointsMarker marker in _pointsMarker)
                {
                    if (marker.Root != null)
                    {
                        for (int i = 0; i < marker.Root.childCount; i++)
                        {
                            Transform point = marker.Root.GetChild(i);
                            point.gameObject.name = $"Spawn point [{i + 1}]";
                        }
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (_spawnPlayerPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_spawnPlayerPoint.position, 2);
            }
            foreach (PointsMarker marker in _pointsMarker)
            {
                Gizmos.color = Color.red;
                if (marker.Root != null)
                {
                    Vector3 previousPos = Vector3.zero;
                    for (int i = 0; i < marker.Root.childCount; i++)
                    {
                        Transform point = marker.Root.GetChild(i);
                        Gizmos.DrawSphere(point.position, 2);
                        if (i != 0)
                        {
                            Gizmos.DrawLine(previousPos, point.position);
                        }
                        previousPos = point.position;
                    }
                }
                if (marker.EndPoint != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(marker.EndPoint.position, 2);
                }
            }
            RefreshName();
        }
    }
}