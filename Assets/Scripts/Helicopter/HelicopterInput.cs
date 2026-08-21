using System;
using UnityEngine;

namespace Assets.Scripts.Helicopter
{
    public class HelicopterInput : MonoBehaviour
    {
        [Serializable]
        public class HelicopterConfig
        {
            [field: Header("Controller")]
            [field: SerializeField] public float MoveAxisTime { get; private set; } = 1;
            [field: SerializeField] public float MoveAxisTimeLowing { get; private set; } = 0.3f;
            [field: SerializeField] public float CollectiveTime { get; private set; } = 3;
            [field: SerializeField] public float CollectiveSpeed { get; private set; } = 500;

            [field: Header("Main rotor")]
            [field: SerializeField] public float LiftForce { get; private set; } = 60;
            [field: SerializeField] public float ForwardLiftForce { get; private set; } = 45;
            [field: SerializeField] public float RightLiftForce { get; private set; } = 25;
            [field: SerializeField] public float Gravity { get; private set; } = -200;

            [field: Header("Tail rotor")]
            [field: SerializeField] public float TailForce { get; private set; } = 0.3f;
            [field: SerializeField] public float TailInputSpeed { get; private set; } = 5;

            [field: Header("Drag")]
            [field: SerializeField] public Vector3 LinearDrag { get; private set; } = new Vector3(2, 2, 2);
            [field: SerializeField] public Vector3 AngularDrag { get; private set; } = new(2, 1, 2.5f);

            [field: Header("Balance")]
            [field: SerializeField] public float BalanceForce { get; private set; } = 5;
            [field: SerializeField] public float IdleYDrag { get; private set; } = 8;

            [field: Header("Effect")]
            [field: SerializeField] public Vector2 EffectSpeedRotate { get; private set; } = new(700, 1100);
            [field: SerializeField] public float EffectCollectiveSmoothSpeed { get; private set; } = 1000;
            [field: SerializeField] public AnimationCurve AudioPitchCurve { get; private set; }
        }
        public HelicopterConfig Config;
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
        public float CurrentCollective { get; private set; }
        public bool IsEngineEnable { get; private set; }

        public Vector2 InputAxis => _currentAxis;
        public float YawRaw => _inputActions.Player.ThrustAndYaw.ReadValue<Vector2>().x;
        public bool IsLift => _inputActions.Player.ThrustAndYaw.ReadValue<Vector2>().y > 0;
        public bool IsLowering => _inputActions.Player.ThrustAndYaw.ReadValue<Vector2>().y < 0;

        private InputSystem_Actions _inputActions;
        private Vector2 _currentAxis;

        private bool _engineEnable = true;
        private void Awake()
        {
            _inputActions = new();
            _inputActions.Enable();
            _inputActions.Player.Sprint.performed += EnableEngine;
        }

        private void OnDestroy()
        {
            _inputActions.Player.Sprint.performed -= EnableEngine;
            _inputActions.Dispose();
        }

        private void Update()
        {
            float collectiveSpeed = Config.CollectiveSpeed / Config.CollectiveTime;
            CurrentCollective = Mathf.MoveTowards(CurrentCollective, _engineEnable ? Config.CollectiveSpeed : 0, collectiveSpeed * Time.deltaTime);
            IsEngineEnable = CurrentCollective > Config.CollectiveSpeed * 0.9f;//compensate for speed

            Vector2 axis = _inputActions.Player.Move.ReadValue<Vector2>();

            float speedX = 1 / (axis.x != 0 ? Config.MoveAxisTime : Config.MoveAxisTimeLowing);
            float speedY = 1 / (axis.y != 0 ? Config.MoveAxisTime : Config.MoveAxisTimeLowing);

            _currentAxis.x = Mathf.MoveTowards(_currentAxis.x, axis.x, speedX * Time.deltaTime);
            _currentAxis.y = Mathf.MoveTowards(_currentAxis.y, axis.y, speedY * Time.deltaTime);
        }

        private void EnableEngine(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            _engineEnable = !_engineEnable;
        }
    }
}