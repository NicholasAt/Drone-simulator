using Assets.Scripts.Data.DronesData;
using UnityEngine;

namespace Assets.Scripts.Drone
{
    public class DroneEffect : MonoBehaviour
    {
        [SerializeField] private Transform[] _leftRotate;
        [SerializeField] private Transform[] _rightRotate;
        [SerializeField] private DroneInput _droneInput;
        [SerializeField] private AudioSource _audioSource;

        private DroneConfig _config;
        private float _currentPitch;

        private void Start()
        {
            _config = _droneInput.Config;
        }

        private void Update()
        {
            float thrust = Mathf.Clamp01(_droneInput.Thrust);
            bool isUp = thrust > 0;

            float speed = isUp ? _config.SpeedUpAudio : _config.SpeedDownAudio;
            float pitch = isUp ? _config.MinMaxAudio.y : _config.MinMaxAudio.x;

            _currentPitch = Mathf.MoveTowards(_currentPitch, pitch, speed * Time.deltaTime);
            _audioSource.pitch = _currentPitch;

            UpdateRotate();
        }

        private void UpdateRotate()
        {
            float pitchLerp = Mathf.InverseLerp(_config.MinMaxAudio.x, _config.MinMaxAudio.y, _currentPitch);
            float rotateSpeed = Mathf.Lerp(_config.EffectSpeedRotate.x, _config.EffectSpeedRotate.y, pitchLerp);

            foreach (Transform left in _leftRotate)
            {
                left.Rotate(0, 0, (rotateSpeed + 100) * Time.deltaTime);//effect
            }
            foreach (Transform right in _rightRotate)
            {
                right.Rotate(0, 0, -rotateSpeed * Time.deltaTime);
            }
        }
    }
}