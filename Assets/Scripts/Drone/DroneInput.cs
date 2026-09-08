using Assets.Scripts.Data.DronesData;
using Assets.Scripts.Services.InputService;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Drone
{
    public class DroneInput : MonoBehaviour
    {
        [field: SerializeField] public DroneConfig Config { get; private set; }
        private IInputService _inputService;

        public Vector2 RollAndPitch => _inputService.RollAndPitch;
        public float Thrust { get; private set; }
        public float Yaw { get; private set; }

        [Inject]
        private void Construct(IInputService inputService)
        {
            _inputService = inputService;
        }
        public void Init(DroneConfig config)
        {
            Config = config;
        }
        private void Update()
        {
            Vector2 thrustAndRoll = _inputService.ThrustAndRoll;
            Thrust = thrustAndRoll.y;
            Yaw = thrustAndRoll.x;
        }
    }
}