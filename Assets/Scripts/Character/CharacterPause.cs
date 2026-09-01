using Assets.Scripts.Services;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Character
{
    public class CharacterPause : MonoBehaviour
    {
        private class PauseData
        {
            public Vector3 AngularVelocity;
            public Vector3 LinearVelocity;
        }

        [SerializeField] private MonoBehaviour[] _disableComponents;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Rigidbody _rigidbody;

        private GameObserver _gameObserver;
        private readonly PauseData _pauseData = new();

        [Inject]
        private void Construct(GameObserver gameObserver)
        {
            _gameObserver = gameObserver;
        }
        private void Awake()
        {
            _gameObserver.OnPauseChange += OnPauseChange;
        }

        private void OnDestroy()
        {
            _gameObserver.OnPauseChange -= OnPauseChange;
        }

        private void OnPauseChange()
        {
            if (_gameObserver.IsPause == false)
            {
                _rigidbody.angularVelocity = _pauseData.AngularVelocity;
                _rigidbody.linearVelocity = _pauseData.LinearVelocity;
            }

            foreach (MonoBehaviour component in _disableComponents)
            {
                component.enabled = !_gameObserver.IsPause;
            }
            _audioSource.enabled = !_gameObserver.IsPause;

            if (_gameObserver.IsPause)
            {
                _pauseData.AngularVelocity = _rigidbody.angularVelocity;
                _pauseData.LinearVelocity = _rigidbody.linearVelocity;

                _rigidbody.angularVelocity = Vector3.zero;
                _rigidbody.linearVelocity = Vector3.zero;
            }
        }
    }
}