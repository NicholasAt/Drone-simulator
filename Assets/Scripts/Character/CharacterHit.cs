using System;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class CharacterHit : MonoBehaviour
    {
        [SerializeField] private TurnedOver _turnedOver;
        [SerializeField] private Rigidbody _rb;

        public Action<float> OnHit { get; set; }
        public Action OnTurned { get; set; }

        private void Start()
        {
            _turnedOver.OnTrigger += () => OnTurned?.Invoke();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (gameObject.activeInHierarchy)
                OnHit?.Invoke(collision.impulse.magnitude);
        }
    }
}