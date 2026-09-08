using Assets.Scripts.Data.Quests;
using Assets.Scripts.Logic;
using Assets.Scripts.Services;
using Assets.Scripts.UI.Windows.Popup;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class DestroyStatic_Quest : BaseQuest<DestroyStatic_Quest.MainConfig>
    {
        [Serializable]
        public class MainConfig : BaseConfig
        {
            public int BestSeconds = 5;
            public int BadSeconds = 15;
        }
        [SerializeField] private Transform _targetsRoot;
        [SerializeField] private Transform _playerInitPoint;
        private GameFactory _gameFactory;
        private UIFactory _uIFactory;
        private QuestObjectsData _questObjectsData;
        private TimerService _timerService;
        private CalculateStarsService _calculateStars;
        private int _currentLives;

        [Inject]
        private void Construct(GameFactory gameFactory, UIFactory uIFactory, QuestObjectsData questObjectsData, TimerService timerService, CalculateStarsService calculateStarsService)
        {
            _gameFactory = gameFactory;
            _uIFactory = uIFactory;
            _questObjectsData = questObjectsData;
            _timerService = timerService;
            _calculateStars = calculateStarsService;
        }

        private void OnValidate()
        {
            RefreshNames();
        }

        protected override async UniTask OnRun()
        {
            await InitObjects();
            _timerService.Start();
        }
        protected override (Vector3 pos, Quaternion rotate) PositionAndRotate()
        {
            return (_playerInitPoint.position, _playerInitPoint.rotation);
        }

        private async UniTask InitObjects()
        {
            for (int i = 0; i < _targetsRoot.childCount; i++)
            {
                Transform point = _targetsRoot.GetChild(i);
                GameObject instance = await _gameFactory.CreateQuestObject(_questObjectsData.DestroyableItemReference, point.position, point.rotation, this.GetCancellationTokenOnDestroy(), "Helicopter");
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
                int stars = _calculateStars.Calculate(Config.BadSeconds, Config.BestSeconds, _timerService.Seconds);
                Win(stars).Forget(Debug.LogError);
            }
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