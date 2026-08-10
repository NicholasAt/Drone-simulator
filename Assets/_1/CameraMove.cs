using UnityEngine;

namespace Assets.Airplane.Scripts
{
    public class CameraMove : MonoBehaviour
    {
        [SerializeField] private bool _moveTarget;
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private float _walking = 15, _running = 100;
        [SerializeField] private float _mouseSpeed = 2f;

        private float _x;
        private float _y;
        private InputSystem_Actions _inputActions;
        private void Start()
        {
            _inputActions = new();
            _inputActions.Enable();
        }
        private void Update()
        {
            if (_moveTarget)
                UpdateMover();
            UpdateRotate();
        }
        private void UpdateMover()
        {
            Vector2 axis = _inputActions.Player.Move.ReadValue<Vector2>();
            Vector3 direction = transform.forward * axis.y + transform.right * axis.x;
            float speed = _inputActions.Player.Sprint.IsPressed() ? _running : _walking;
            _target.position += speed * Time.deltaTime * direction;
        }
        private void UpdateRotate()
        {
            Vector2 input = _inputActions.Player.Look.ReadValue<Vector2>() * _mouseSpeed;
            _x += input.x;
            _y -= input.y;
            Quaternion rotate = Quaternion.Euler(_y, _x, 0);
            Vector3 pos = rotate * _offset + _target.position;

            transform.SetPositionAndRotation(pos, rotate);
        }
    }
}