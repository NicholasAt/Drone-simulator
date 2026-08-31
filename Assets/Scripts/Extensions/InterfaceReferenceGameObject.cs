using System;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    [Serializable]
    public class InterfaceReferenceGameObject<T> where T : class
    {
        [SerializeField] private GameObject _target;

        private T _value;

        public T Value
        {
            get
            {
                if (_value == null && _target != null)
                    _value = _target.GetComponent<T>();

                return _value;
            }
        }

        public void OnValidate()
        {
            if (_target != null && _target.GetComponent<T>() == null)
            {
                _target = null;
                _value = null;
            }
        }
    }
}