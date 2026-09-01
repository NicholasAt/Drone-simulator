using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Bots
{
    public class BotMovementByArea : MonoBehaviour, IRefreshPositions
    {
        [SerializeField] private NavMeshAgent _agent;
        private Transform _matrix;
        private Vector3 _size;
        private Vector3 _currentTarget;
        private bool _setPoint;

        public void Init(float speed)
        {
            _agent.speed = speed;
        }

        private void OnEnable()
        {
            if (_setPoint)
                _agent.SetDestination(_currentTarget);
        }

        private void Update()
        {
            if (_agent.enabled)
            {
                if (Vector3.Distance(transform.position, _currentTarget) < _agent.radius)
                    NewPosition();
            }
        }

        void IRefreshPositions.Show(Vector3 pos, Quaternion rotate)
        {
            gameObject.SetActive(true);
            transform.SetPositionAndRotation(pos, rotate);
            _agent.enabled = true;
            NewPosition();
        }

        void IRefreshPositions.Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetArea(Transform matrix, Vector3 size)
        {
            _matrix = matrix;
            _size = size;
            _agent.enabled = true;
            NewPosition();
        }
        private void NewPosition()
        {
            _currentTarget = GetRandomPos();
            _agent.SetDestination(_currentTarget);
            _setPoint = true;
        }
        private Vector3 GetRandomPos()
        {
            Vector3 halfSize = _size / 2;
            Vector3 pos = new Vector3()
            {
                x = Random.Range(-halfSize.x, halfSize.x),
                y = 0,
                z = Random.Range(-halfSize.z, halfSize.z),
            };

            return _matrix.TransformPoint(pos);
        }
    }
}