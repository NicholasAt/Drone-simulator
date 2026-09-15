using Assets.Scripts.Character;
using Assets.Scripts.Services;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Logic
{
    public class OutsideMapReporter : MonoBehaviour
    {
        private TriggerReporter[] _reporters;
        private GameObserver _gameObserver;

        [Inject]
        private void Construct(GameObserver gameObserver)
        {
            _gameObserver = gameObserver;
        }
        private void Awake()
        {
            _reporters = GetComponentsInChildren<TriggerReporter>();
            foreach (TriggerReporter reporter in _reporters)
            {
                reporter.OnTrigger += OnTrigger;
            }
        }
        private void OnDestroy()
        {
            foreach (TriggerReporter reporter in _reporters)
            {
                reporter.OnTrigger -= OnTrigger;
            }
        }
        private void OnTrigger(GameObject @object)
        {
            CharacterMarker marker = @object.GetComponentInParent<CharacterMarker>();
            if (marker != null && marker.IsBot == false)
            {
                _gameObserver.SendOutside();
            }
        }
    }
}