using UnityEngine;

namespace Assets.Scripts.Helicopter
{
    public class MainRotor : MonoBehaviour
    {
        private const int FlyPoints = 4;

        [SerializeField] private HelicopterInput _helicopter;
        [SerializeField] private Transform _forward, _back, _left, _right;

        private Rigidbody _rb;

        private void Start()
        {
            _rb = _helicopter.Rigidbody;
        }

        private void FixedUpdate()
        {
            HelicopterInput.HelicopterConfig config = _helicopter.Config;

            float gravity = -config.Gravity;
            _rb.AddForce(Vector3.down * gravity);

            if (_helicopter.IsEngineEnable == false)
                return;

            float pitch = _helicopter.InputAxis.y;
            float roll = _helicopter.InputAxis.x;
            bool isLift = _helicopter.IsLift;
            bool isLowering = _helicopter.IsLowering;

            if (pitch > 0)
                _rb.AddForceAtPosition(pitch * config.ForwardLiftForce * _back.up, _back.position);
            else
                _rb.AddForceAtPosition(-pitch * config.ForwardLiftForce * _forward.up, _forward.position);

            if (roll > 0)
                _rb.AddForceAtPosition(roll * config.RightLiftForce * _left.up, _left.position);
            else
                _rb.AddForceAtPosition(-roll * config.RightLiftForce * _right.up, _right.position);

            if (isLift)
            {
                _rb.AddForce(config.LiftForce * transform.up);
            }
            else if (isLowering)
            {
                _rb.AddForce(config.LiftForce * -transform.up);
            }

            float balanceForce = gravity;
            balanceForce /= FlyPoints;
            _rb.AddForceAtPosition(balanceForce * _forward.up, _forward.position);
            _rb.AddForceAtPosition(balanceForce * _back.up, _back.position);
            _rb.AddForceAtPosition(balanceForce * _left.up, _left.position);
            _rb.AddForceAtPosition(balanceForce * _right.up, _right.position);

            _rb.AddForce(Mathf.Abs(pitch * config.ForwardLiftForce) * -transform.up);
            _rb.AddForce(Mathf.Abs(roll * config.RightLiftForce) * -transform.up);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_forward.position, 0.5f);
            Gizmos.DrawSphere(_back.position, 0.5f);
            Gizmos.DrawSphere(_left.position, 0.5f);
            Gizmos.DrawSphere(_right.position, 0.5f);
        }
    }
}