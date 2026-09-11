using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class CircleCreator : MonoBehaviour
    {
        [SerializeField] private int _segments = 15;
        [SerializeField] private float _radius = 15;
        [SerializeField] private float _width = 0.6f;
        [SerializeField] private float _lenght = 6.3f;

        [ContextMenu("Run")]
        private void Run()
        {
            while (transform.childCount > 0)
            {
                GameObject o = transform.GetChild(transform.childCount - 1).gameObject;
                DestroyImmediate(o);
            }

            for (int i = 0; i <= _segments; i++)
            {
                float rad = i * (Mathf.PI * 2) / _segments;
                Vector3 circle = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * _radius;

                Transform instance = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                instance.localScale = new Vector3(_lenght, _width, _width);
                instance.position = circle;
                instance.up = Vector3.zero - circle;
                instance.SetParent(transform);
            }
        }
    }
}