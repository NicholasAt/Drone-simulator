using System;
using UnityEngine;

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
        }
        public DroneConfig Config;
        private InputSystem_Actions _inputActions;

        public Vector2 RollAndPitch { get; private set; }
        public float Thrust { get; private set; }
        public float Yaw { get; private set; }

        private void Awake()
        {
            _inputActions = new();
            _inputActions.Enable();
        }

        private void Update()
        {
            RollAndPitch = _inputActions.Player.Move.ReadValue<Vector2>();
            Vector2 thrustAndRoll = _inputActions.Player.ThrustAndYaw.ReadValue<Vector2>();
            Thrust = thrustAndRoll.y;
            Yaw = thrustAndRoll.x;
        }
    }
}