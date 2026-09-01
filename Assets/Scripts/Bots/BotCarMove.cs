using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Bots
{
    public class BotCarMove : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        private Vector3 _point;
        private bool _setPoint;

        public void Init(float speed)
        {
            _agent.speed = speed;
        }

        private void OnEnable()
        {
            if (_setPoint)
                _agent.SetDestination(_point);
        }

        public void SetPoint(Vector3 point)
        {
            _point = point;
            _agent.enabled = true;
            _agent.SetDestination(_point);
            _setPoint = true;
        }
    }
}