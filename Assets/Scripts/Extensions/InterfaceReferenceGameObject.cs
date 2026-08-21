using System;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    [Serializable]
    public class InterfaceReferenceGameObject<T> where T : class
    {
        [SerializeField] private GameObject _target;
        public T Value => _value ??= _value = _target.GetComponent<T>();
        private T _value;
        public void OnValidate()
        {
            if (_target != null)
            {
                if (_target.TryGetComponent<T>(out _) == false)
                    _target = null;
            }
        }
    }
}