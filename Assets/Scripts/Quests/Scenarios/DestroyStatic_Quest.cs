using Assets.Scripts.Data.Quests;
using Assets.Scripts.Logic;
using Assets.Scripts.Services;
using Assets.Scripts.Services.CameraService;
using Assets.Scripts.Services.GameProgress;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class DestroyStatic_Quest : BaseQuest
    {
        [Serializable]
        private class Config
        {
            public float DieImpulse = 20;
            public float DamageRadius = 5;
        }
        [SerializeField] private Config _config;
        [SerializeField] private Transform _targetsRoot;
        [SerializeField] private Transform _playerInitPoint;
        private TransportFactory _transportFactory;

        private GameFactory _gameFactory;
        private CameraStateService _cameraService;
        private HitHandler _hitHandler;
        private QuestObjectsData _questObjectsData;
        private TempLevelProgress _levelProgress;
        private CancellationToken _ct;
        private int _currentLives;

        [Inject]
        private void Construct(GameFactory gameFactory, TransportFactory transportFactory, ProgressService progressService, CameraStateService cameraService, HitHandler hitHandler, QuestObjectsData questObjectsData)
        {
            _gameFactory = gameFactory;
            _transportFactory = transportFactory;
            _cameraService = cameraService;
            _hitHandler = hitHandler;
            _questObjectsData = questObjectsData;
            _levelProgress = progressService.TempLevelProgress;
        }
        private void OnValidate()
        {
            RefreshNames();
        }
        private void OnDestroy()
        {
            if (_transportFactory.PlayerKeeper != null)
            {
                _transportFactory.PlayerKeeper.CharacterHit.OnHit -= OnPlayerHit;
                _transportFactory.PlayerKeeper.CharacterHit.OnTurned -= OnPlayerTurned;
            }
        }
        protected override async UniTask OnRun()
        {
            _ct = this.GetCancellationTokenOnDestroy();
            _hitHandler.Init(_config.DamageRadius);
            await _transportFactory.CreateTransport(_levelProgress.QuestID, _playerInitPoint.position, _playerInitPoint.rotation);
            await InitObjects();
            _transportFactory.PlayerKeeper.CharacterHit.OnHit += OnPlayerHit;
            _transportFactory.PlayerKeeper.CharacterHit.OnTurned += OnPlayerTurned;
        }

        private async UniTask InitObjects()
        {
            for (int i = 0; i < _targetsRoot.childCount; i++)
            {
                Transform point = _targetsRoot.GetChild(i);
                GameObject instance = await _gameFactory.CreateQuestObject(_questObjectsData.DestroyableItemReference, point.position, point.rotation, _ct);
                if (instance.TryGetComponent(out IApplyDamage applyDamage) == false)
                    Debug.LogError("no damage");

                applyDamage.Happened += OnDestroyObject;
                _currentLives++;
            }
        }

        private void OnDestroyObject()
        {
            _currentLives--;
            if (_currentLives <= 0)
            {
                Win();
            }
        }
        private void Win()
        {
            Debug.LogError("win");
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

        private Vector3 CharacterPos() =>
            _transportFactory.PlayerKeeper.Pos();

        private void PlayAnimation()
        {
            _cameraService.Show(CharacterPos(), RestartPlayer, _ct).Forget(Debug.LogError);
            _transportFactory.PlayerKeeper.CharacterRefresher.Hide();
        }

        private void RestartPlayer()
        {
            _transportFactory.PlayerKeeper.CharacterRefresher.Show(_playerInitPoint.position, _playerInitPoint.rotation);
            _cameraService.SetParent(_transportFactory.PlayerKeeper.Character.transform);
        }
        private void RefreshNames()
        {
            if (_targetsRoot != null)
            {
                for (int i = 0; i < _targetsRoot.childCount; i++)
                {
                    _targetsRoot.GetChild(i).gameObject.name = $"Point [{i + 1}]";
                }
            }
        }
        private void OnDrawGizmos()
        {
            if (_playerInitPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_playerInitPoint.position, 2);
            }
            if (_targetsRoot != null)
            {
                Gizmos.color = Color.red;

                for (int i = 0; i < _targetsRoot.childCount; i++)
                {
                    Vector3 pos = _targetsRoot.GetChild(i).position;
                    Gizmos.DrawSphere(pos, 2);
                }
            }
            RefreshNames();
        }
    }
}