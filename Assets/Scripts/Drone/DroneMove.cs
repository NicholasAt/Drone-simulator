using Assets.Scripts.Data.DronesData;
using Assets.Scripts.Services;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Drone
{
    public class DroneMove : MonoBehaviour
    {
        private const int Propellers = 4;
        [SerializeField] private DroneInput _droneInput;
        [SerializeField] private Rigidbody _body;
        [SerializeField] private Transform _fLeft, _fRight, _bLeft, _bRight;
        private DroneConfig _config;
        private GameObserver _gameObserver;

        [Inject]
        private void Construct(GameObserver gameObserver)
        {
            _gameObserver = gameObserver;
        }
        private void Start()
        {
            _config = _droneInput.Config;
        }

        private void FixedUpdate()
        {
            _body.AddForce(new Vector3(0, _config.Gravity, 0));

            if (_droneInput.Thrust > 0)
            {
                float force = _config.Force / Propellers * _droneInput.Thrust;

                _body.AddForceAtPosition(_fLeft.up * force, _fLeft.position);
                _body.AddForceAtPosition(_fRight.up * force, _fRight.position);
                _body.AddForceAtPosition(_bLeft.up * force, _bLeft.position);
                _body.AddForceAtPosition(_bRight.up * force, _bRight.position);
            }

            UpdatePitchAndRoll();
            UpdateYaw();
            UpdateLinearDrag();
            UpdateAngularDrag();
            ShowSpeed();
        }
        private void OnDisable()
        {
            _gameObserver.SetCharacterSpeed(0);
        }
        private void ShowSpeed()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(_body.linearVelocity);
            _gameObserver.SetCharacterSpeed(localVelocity.magnitude);
        }
        private void UpdateYaw()
        {
            float force = _droneInput.Yaw * _config.YawForce;
            _body.AddRelativeTorque(0, force, 0, ForceMode.Acceleration);
        }

        private void UpdatePitchAndRoll()
        {
            Vector2 axis = _droneInput.RollAndPitch * _config.PitchForce;
            _body.AddForceAtPosition(_fLeft.up * -axis.y, _fLeft.position);
            _body.AddForceAtPosition(_fRight.up * -axis.y, _fRight.position);

            _body.AddForceAtPosition(_fLeft.up * axis.x, _fLeft.position);
            _body.AddForceAtPosition(_bLeft.up * axis.x, _bLeft.position);
        }

        private void UpdateAngularDrag()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(_body.angularVelocity);
            Vector3 drag = new(
                -localVelocity.x * _config.AngularDrag.x,
                -localVelocity.y * _config.AngularDrag.y,
                -localVelocity.z * _config.AngularDrag.z);

            _body.AddRelativeTorque(drag);
        }

        private void UpdateLinearDrag()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(_body.linearVelocity);
            Vector3 drag = new Vector3(
                -localVelocity.x * _config.Drag.x,
                -(localVelocity.y * 0.5f) * _config.Drag.y,
                -localVelocity.z * _config.Drag.z);

            _body.AddRelativeForce(drag);
        }
    }
}