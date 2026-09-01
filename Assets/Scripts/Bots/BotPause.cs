using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Assets.Scripts.Bots
{
    public class BotPause : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] _disableComponents;
        [SerializeField] private NavMeshAgent _agent;
        private GameObserver _gameObserver;

        [Inject]
        private void Construct(GameObserver gameObserver)
        {
            _gameObserver = gameObserver;
        }

        private void Awake()
        {
            _gameObserver.OnPauseChange += OnChangePause;
        }

        private void OnDestroy()
        {
            _gameObserver.OnPauseChange -= OnChangePause;
        }

        private void OnChangePause()
        {
            if (_gameObserver.IsPause == false)
                _agent.enabled = true;

            foreach (MonoBehaviour component in _disableComponents)
            {
                component.enabled = !_gameObserver.IsPause;
            }

            if (_gameObserver.IsPause)
                _agent.enabled = false;
        }
    }
}