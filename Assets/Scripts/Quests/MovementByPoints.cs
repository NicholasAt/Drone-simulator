using Assets.Scripts.Interactive;
using Assets.Scripts.Services;
using Assets.Scripts.Services.GameProgress;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests
{
    public class MovementByPoints : MonoBehaviour
    {
        public Action<Transform> OnTriggered { get; set; }
        public Action OnFinish { get; set; }

        [SerializeField] private Transform _initPoint;
        [SerializeField] private Transform _movePointsRoot;
        private readonly List<Transform> _movePoints = new();

        private GameFactory _gameFactory;
        private TempLevelProgress _levelProgress;
        private int _currentPointIndex = -1;
        private bool _isEnd;

        [Inject]
        private void Construct(GameFactory gameFactory, ProgressService progressService)
        {
            _gameFactory = gameFactory;
            _levelProgress = progressService.TempLevelProgress;
        }
        private void OnValidate()
        {
            RefreshNames();
        }

        public async UniTask Run()
        {
            InitPoints();
            await NextPoint();
        }
        public bool IsLastPoint()
        {
            return _movePoints.Count <= _currentPointIndex + 1;
            
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
                Transform movePoint = CurrentPoint();
                GameObject questPoint = await _gameFactory.CreateQuestPoint(movePoint.position, movePoint.rotation);

                if (questPoint.TryGetComponent(out TriggerReporter triggerReporter) == false)
                    Debug.LogError("no reporter");

                triggerReporter.OnTrigger += (target) => OnTrigger(triggerReporter, target).Forget();
            }
        }
        private Transform CurrentPoint()
        {
            return _movePoints[_currentPointIndex];
        }
        private void InitPoints()
        {
            for (int i = 0; i < _movePointsRoot.childCount; i++)
            {
                _movePoints.Add(_movePointsRoot.GetChild(i));
            }
        }

        private void EndQuest()
        {
            if (_isEnd)
                return;

            _isEnd = true;
            OnFinish?.Invoke();
        }
       
        private async UniTask OnTrigger(TriggerReporter reporter, GameObject target)
        {
            CharacterMarker character = target.GetComponentInParent<CharacterMarker>();
            if (character == false)
                return;

            reporter.OnTrigger = null;
            Destroy(reporter.gameObject);
            OnTriggered?.Invoke(CurrentPoint());
            await NextPoint();
        }
        private void RefreshNames()
        {
            if (_movePointsRoot != null)
            {
                for (int i = 0; i < _movePointsRoot.childCount; i++)
                {
                    _movePointsRoot.GetChild(i).gameObject.name = $"Point [{i + 1}]";
                }
            }
        }
        private void OnDrawGizmos()
        {
            if (_initPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_initPoint.position, 15);
            }

            if (_movePointsRoot != null)
            {
                const int frequency = 25;
                const float arrowSize = 7;
                const float angle = 35;

                Gizmos.color = Color.red;
                Vector3 previousPos = Vector3.zero;
                for (int i = 0; i < _movePointsRoot.childCount; i++)
                {
                    Transform point = _movePointsRoot.GetChild(i);
                    Gizmos.DrawSphere(point.position, 5);
                    if (i != 0)
                    {
                        Gizmos.DrawLine(previousPos, point.position);

                        Vector3 dir = previousPos - point.position;
                        Vector3 right = Quaternion.AngleAxis(angle, Vector3.Cross(dir.normalized, Vector3.up)) * (dir.normalized * arrowSize);
                        Vector3 left = Quaternion.AngleAxis(-angle, Vector3.Cross(dir.normalized, Vector3.up)) * (dir.normalized * arrowSize);
                        Vector3 up = Quaternion.AngleAxis(angle, Vector3.Cross(dir.normalized, Vector3.right)) * (dir.normalized * arrowSize);
                        Vector3 down = Quaternion.AngleAxis(-angle, Vector3.Cross(dir.normalized, Vector3.right)) * (dir.normalized * arrowSize);

                        int distance = Mathf.RoundToInt(dir.magnitude);

                        for (int j = 0; j < distance; j++)
                        {
                            if (j % frequency != frequency - 1)
                                continue;

                            float size = j;
                            Vector3 pos = previousPos - dir.normalized * size;
                            Gizmos.DrawRay(pos, right);
                            Gizmos.DrawRay(pos, left);
                            Gizmos.DrawRay(pos, up);
                            Gizmos.DrawRay(pos, down);
                        }
                    }
                    previousPos = point.position;
                }
            }
            RefreshNames();
        }
    }
}