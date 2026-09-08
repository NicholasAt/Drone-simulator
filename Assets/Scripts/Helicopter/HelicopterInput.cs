using Assets.Scripts.Data.HelicoptersData;
using Assets.Scripts.Services.InputService;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Helicopter
{
    public class HelicopterInput : MonoBehaviour
    {
        [field: SerializeField] public HelicopterConfig Config { get; private set; }
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
        public float CurrentCollective { get; private set; }
        public bool IsEngineEnable { get; private set; }

        public Vector2 InputAxis => _currentAxis;
        public float YawRaw => _inputService.ThrustAndRoll.x;
        public bool IsLift => _inputService.ThrustAndRoll.y > 0;
        public bool IsLowering => _inputService.ThrustAndRoll.y < 0;

        private Vector2 _currentAxis;

        private IInputService _inputService;
        private Rigidbody _rb;
        private bool _engineEnable = true;

        [Inject]
        private void Constuct(IInputService inputService)
        {
            _inputService = inputService;
        }
        public void Init(HelicopterConfig config)
        {
            Config = config;
        }
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            CurrentCollective = Config.CollectiveSpeed;
        }
        private void Update()
        {
            float collectiveSpeed = Config.CollectiveSpeed / Config.CollectiveTime;
            CurrentCollective = Mathf.MoveTowards(CurrentCollective, _engineEnable ? Config.CollectiveSpeed : 0, collectiveSpeed * Time.deltaTime);
            IsEngineEnable = CurrentCollective > Config.CollectiveSpeed * 0.9f;//compensate for speed

            Vector2 axis = _inputService.MoveAxis;

            float speedX = 1 / (axis.x != 0 ? Config.MoveAxisTime : Config.MoveAxisTimeLowing);
            float speedY = 1 / (axis.y != 0 ? Config.MoveAxisTime : Config.MoveAxisTimeLowing);

            _currentAxis.x = Mathf.MoveTowards(_currentAxis.x, axis.x, speedX * Time.deltaTime);
            _currentAxis.y = Mathf.MoveTowards(_currentAxis.y, axis.y, speedY * Time.deltaTime);
        }
    }
}