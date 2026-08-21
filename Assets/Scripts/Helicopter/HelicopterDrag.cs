using UnityEngine;

namespace Assets.Scripts.Helicopter
{
    public class HelicopterDrag : MonoBehaviour
    {
        [SerializeField] private HelicopterInput _helicopterController;

        private Rigidbody _rb;
        private HelicopterInput.HelicopterConfig _config;

        private void Start()
        {
            _rb = _helicopterController.Rigidbody;
            _config = _helicopterController.Config;
        }

        private void FixedUpdate()
        {
            UpdateBalance();
            UpdateAngularDrag();

            if (_helicopterController.IsEngineEnable)
                UpdateLinearDrag();
        }

        private void UpdateLinearDrag()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(_rb.linearVelocity);
            Vector3 localDrag = new(
                -localVelocity.x * _config.LinearDrag.x,
                -localVelocity.y * _config.LinearDrag.y,
                -localVelocity.z * _config.LinearDrag.z
                );

            _rb.AddRelativeForce(localDrag);
            //if (_helicopterController.IsLift)
            //{
            //    if (_helicopterController.InputAxis.magnitude != 0)
            //    {
            //        float move = Mathf.Clamp01(Mathf.Abs(_helicopterController.InputAxis.magnitude));
            //        float drag = _rb.linearVelocity.y * _config.IdleYDrag;
            //        _rb.AddForce(move * drag * -transform.up);
            //    }
            //}

            //else if (_helicopterController.IsLift == false && _helicopterController.IsLowering == false)
            //{
            //    float value = _rb.linearVelocity.y * _config.IdleYDrag;
            //    _rb.AddForce(value * -transform.up);
            //}
        }

        private void UpdateAngularDrag()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(_rb.angularVelocity);

            Vector3 localDrag = new(
                -localVelocity.x * _config.AngularDrag.x,
                -localVelocity.y * _config.AngularDrag.y,
                -localVelocity.z * _config.AngularDrag.z);

            _rb.AddRelativeTorque(localDrag, ForceMode.Acceleration);
        }
        private void UpdateBalance()
        {
            Vector3 angles = Vector3.Cross(Vector3.up, transform.up);
            _rb.AddTorque(_config.BalanceForce * -angles, ForceMode.Acceleration);
        }
    }
}