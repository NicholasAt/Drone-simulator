using Assets.Scripts.Interactive;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public class DroneMove_Quest : BaseQuest
    {
        [SerializeField] private Transform _initPoint;
        [SerializeField] private List<Transform> _movePoints;

        private GameFactory _gameFactory;
        private int _currentPointIndex = -1;
        private bool _isEnd;
        [Inject]
        private void Construct(GameFactory gameFactory)
        {
            _gameFactory = gameFactory;
        }

        protected override async UniTask OnRun()
        {
            GameObject drone = await _gameFactory.CreateDrone(_initPoint.position, _initPoint.rotation);
            Camera.main.transform.SetParent(drone.transform, false);

            await NextPoint();
        }
        private async UniTask NextPoint()
        {
            _currentPointIndex++;
            if (_movePoints.Count <= _currentPointIndex)
            {
                EndQuest();
            }
            else
            {
                Transform movePoint = _movePoints[_currentPointIndex];
                GameObject questPoint = await _gameFactory.CreateQuestPoint(movePoint.position, movePoint.rotation);

                if (questPoint.TryGetComponent(out TriggerReporter triggerReporter) == false)
                    Debug.LogError("no reporter");

                triggerReporter.OnTrigger += (target) => OnTrigger(triggerReporter, target).Forget();
            }
        }
        private void EndQuest()
        {
            if (_isEnd)
                return;

            _isEnd = true;
            Debug.LogError("end");
        }
        private async UniTask OnTrigger(TriggerReporter reporter, GameObject target)
        {
            CharacterMarker character = target.GetComponentInParent<CharacterMarker>();
            if (character == false)
                return;

            reporter.OnTrigger = null;
            Destroy(reporter.gameObject);
            await NextPoint();
        }


        private void OnDrawGizmos()
        {
            if (_initPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_initPoint.position, 15);
            }
        }
    }
}