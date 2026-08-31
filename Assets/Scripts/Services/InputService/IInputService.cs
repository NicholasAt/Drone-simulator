using System;
using UnityEngine;

namespace Assets.Scripts.Services.InputService
{
    public interface IInputService
    {
        Vector2 RollAndPitch { get; }
        Vector2 ThrustAndRoll { get; }
        Vector2 MoveAxis { get; }

        void Init();
    }
    public class InputService : IInputService
    {
        private InputSystem_Actions _inputActions;

        public Vector2 RollAndPitch => _inputActions.Player.Move.ReadValue<Vector2>();
        public Vector2 ThrustAndRoll => _inputActions.Player.ThrustAndYaw.ReadValue<Vector2>();
        public Vector2 MoveAxis => _inputActions.Player.Move.ReadValue<Vector2>();

        public void Init()
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Enable();
        }
    }
}