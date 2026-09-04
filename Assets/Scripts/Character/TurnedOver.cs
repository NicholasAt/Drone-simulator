using System;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class TurnedOver : MonoBehaviour
    {
        public Action OnTrigger { get; set; }
        private void OnTriggerEnter(Collider other)
        {
            if (gameObject.activeInHierarchy && other.isTrigger == false)
                OnTrigger?.Invoke();
        }
    }
}