using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Bots
{
    public class BotCarMove : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        private Vector3 _point;

        public void Init(float speed)
        {
            _agent.speed = speed;
        }

        public void SetPoint(Vector3 point)
        {
            _point = point;
            _agent.enabled = true;
            _agent.SetDestination(_point);
        }
    }
}