using Assets.Scripts.Services.InputService;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Drone
{
    public class DroneInput : MonoBehaviour
    {
        [Serializable]
        public class DroneConfig
        {
            [field: SerializeField] public float PitchForce { get; private set; } = 4;
            [field: SerializeField] public float YawForce { get; private set; } = 15;
            [field: SerializeField] public float Force { get; private set; } = 140;
            [field: SerializeField] public float Gravity { get; private set; } = -50;
            [field: SerializeField] public Vector3 Drag { get; private set; } = new(0.7f, 2, 0.7f);
            [field: SerializeField] public Vector3 AngularDrag { get; private set; } = new(2, 1.5f, 2);

            [field: Header("Effect")]
            [field: SerializeField] public Vector2 MinMaxAudio { get; private set; } = new(0.5f, 1.1f);
            [field: SerializeField] public float SpeedUpAudio { get; private set; } = 3;
            [field: SerializeField] public float SpeedDownAudio { get; private set; } = 1;
            [field: SerializeField] public Vector2 EffectSpeedRotate { get; private set; } = new(700, 1100);
        }
        public DroneConfig Config;
        private IInputService _inputService;

        public Vector2 RollAndPitch => _inputService.RollAndPitch;
        public float Thrust { get; private set; }
        public float Yaw { get; private set; }

        [Inject]
        private void Construct(IInputService inputService)
        {
            _inputService = inputService;
        }

        private void Update()
        {
            Vector2 thrustAndRoll = _inputService.ThrustAndRoll;
            Thrust = thrustAndRoll.y;
            Yaw = thrustAndRoll.x;
        }
    }
}