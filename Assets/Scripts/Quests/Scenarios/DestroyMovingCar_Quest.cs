using Assets.Scripts.Bots;
using Assets.Scripts.Character;
using Assets.Scripts.Data.BotsData.CarData;
using Assets.Scripts.Logic;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class DestroyMovingCar_Quest : BaseQuest
    {
        [Serializable]
        public class PointsMarker
        {
            public Transform Root;
            public Transform EndPoint;
            public CarID CarID;
        }
        [SerializeField] private TriggerReporter _loseReporter;
        [SerializeField] private Transform _spawnPlayerPoint;
        [SerializeField] private List<PointsMarker> _pointsMarker;

        private GameFactory _gameFactory;
        private bool _isEnd;

        [Inject]
        private void Construct(GameFactory gameFactory)
        {
            _gameFactory = gameFactory;
        }
        private void OnValidate()
        {
            RefreshName();
        }
        protected override async UniTask OnRun()
        {
            _loseReporter.OnTrigger += OnTrigger;
            await _gameFactory.CreateTransport(_spawnPlayerPoint.position, _spawnPlayerPoint.rotation);
            await InitCar();
        }

        private async UniTask InitCar()
        {
            foreach (PointsMarker marker in _pointsMarker)
            {
                for (int i = 0; i < marker.Root.childCount; i++)
                {
                    Transform point = marker.Root.GetChild(i);
                    BotCarMove car = await _gameFactory.CreateCar(marker.CarID, point.position, point.rotation);
                    car.SetPoint(marker.EndPoint.position);
                }
            }
        }
        private void OnTrigger(GameObject obj)
        {
            CharacterMarker cahracter = obj.GetComponentInParent<CharacterMarker>();
            if (cahracter != null && cahracter.IsBot)
                Lose();
        }

        private void Lose()
        {
            if (_isEnd)
                return;
            _isEnd = true;

            Debug.LogError("Lose");
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