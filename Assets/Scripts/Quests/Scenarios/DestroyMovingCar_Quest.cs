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
using System.Threading;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class DestroyMovingCar_Quest : BaseQuest
    {
        [Serializable]
        private class PointsMarker
        {
            public Transform Root;
            public Transform EndPoint;
            public CarID CarID;
        }
        [Serializable]
        private class Config
        {
            public float PlayerDamageRadius = 5;
            public float DieImpulse = 20;
           [TextArea] public string WinMessage;
           [TextArea] public string LoseMessage;
        }
        [SerializeField] private Config _config;
        [SerializeField] private List<PointsMarker> _pointsMarker;
        [SerializeField] private TriggerReporter _loseReporter;
        [SerializeField] private Transform _spawnPlayerPoint;
        private TransportFactory _transportFactory;
        private CameraStateService _cameraService;
        private HitHandler _hitHandler;
        private TempLevelProgress _levelProgress;
        private GameFactory _gameFactory;
        private CancellationToken _ct;

        private int _currentCarrs;
        [Inject]
        private void Construct(GameFactory gameFactory, TransportFactory transportFactory, ProgressService progressService, CameraStateService cameraService, HitHandler hitHandler)
        {
            _gameFactory = gameFactory;
            _transportFactory = transportFactory;
            _cameraService = cameraService;
            _hitHandler = hitHandler;
            _levelProgress = progressService.TempLevelProgress;
        }
        private void OnValidate()
        {
            RefreshName();
        }
        protected override async UniTask OnRun()
        {
            _ct = this.GetCancellationTokenOnDestroy();
            _hitHandler.Init(_config.PlayerDamageRadius);
            await _transportFactory.CreateTransport(_levelProgress.QuestID, _spawnPlayerPoint.position, _spawnPlayerPoint.rotation);
            await InitCars();

            _transportFactory.PlayerKeeper.CharacterHit.OnHit += OnPlayerHit;
            _transportFactory.PlayerKeeper.CharacterHit.OnTurned += OnPlayerTurned;
            _loseReporter.OnTrigger += OnLose;
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

        private async UniTask InitCars()
        {
            foreach (PointsMarker marker in _pointsMarker)
            {
                for (int i = 0; i < marker.Root.childCount; i++)
                {
                    Transform point = marker.Root.GetChild(i);
                    BotCarMove car = await _gameFactory.CreateCar(marker.CarID, point.position, point.rotation, _ct);
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
                ProtectedWin(_config.WinMessage).Forget(Debug.LogError);
        }

        private Vector3 CharacterPos()
        {
            return _transportFactory.PlayerKeeper.Pos();
        }
        private void PlayAnimation()
        {
            _cameraService.Show(CharacterPos(), RestartPlayer, _ct).Forget(Debug.LogError);
            _transportFactory.PlayerKeeper.CharacterRefresher.Hide();
        }
        private void RestartPlayer()
        {
            _transportFactory.PlayerKeeper.CharacterRefresher.Show(_spawnPlayerPoint.position, _spawnPlayerPoint.rotation);
            _cameraService.SetParent(_transportFactory.PlayerKeeper.Character.transform);
        }

        private void OnLose(GameObject obj)
        {
            CharacterMarker cahracter = obj.GetComponentInParent<CharacterMarker>();
            if (cahracter != null && cahracter.IsBot)
                ProtectedLose(_config.LoseMessage).Forget(Debug.LogError);
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