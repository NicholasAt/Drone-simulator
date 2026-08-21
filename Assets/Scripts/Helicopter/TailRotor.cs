using UnityEngine;

namespace Assets.Scripts.Helicopter
{
    public class TailRotor : MonoBehaviour
    {
        [SerializeField] private HelicopterInput _helicopterController;
        [SerializeField] private Transform _rotorPoint;

        [SerializeField] private Vector3 _gizmos = new(0.3f, 1.5f, 0.3f);

        private Rigidbody _rb;
        private float _targetYaw;

        private void Start()
        {
            _rb = _helicopterController.Rigidbody;
        }
        private void FixedUpdate()
        {
            if (_helicopterController.IsEngineEnable == false)
                return;

            HelicopterInput.HelicopterConfig config = _helicopterController.Config;

            float yaw = _helicopterController.YawRaw;
            _targetYaw = Mathf.Lerp(_targetYaw, yaw, config.TailInputSpeed * Time.fixedDeltaTime);
            _rb.AddForceAtPosition(_targetYaw * config.TailForce * -_rotorPoint.right, _rotorPoint.position, ForceMode.Acceleration);
        }
        private void OnDrawGizmos()
        {
            if (_rotorPoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.matrix = _rotorPoint.localToWorldMatrix;
                Gizmos.DrawWireCube(Vector3.zero, _gizmos);
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(_gizmos.x, _gizmos.z, _gizmos.y));
            }
        }
    }
}