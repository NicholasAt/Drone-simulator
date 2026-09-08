using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class RotateObject : MonoBehaviour
    {
        [SerializeField] private float _speed = 1000;
        [SerializeField] private Transform[] _xObjects;
        [SerializeField] private Transform[] _yObjects;

        private void Update()
        {
            float speed = _speed * Time.deltaTime;
            foreach (Transform x in _xObjects)
            {
                x.Rotate(speed, 0, 0);
            }
            foreach (Transform y in _yObjects)
            {
                y.Rotate(0, speed, 0);
            }
        }
    }
}