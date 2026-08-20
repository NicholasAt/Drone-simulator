using UnityEngine;

namespace Assets.Scripts.Drone
{
    public class DroneEffect : MonoBehaviour
    {
        [SerializeField] private DroneInput _droneInput;
        [SerializeField] private AudioSource _audioSource;

        private DroneInput.DroneConfig _config;
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
        }
    }
}