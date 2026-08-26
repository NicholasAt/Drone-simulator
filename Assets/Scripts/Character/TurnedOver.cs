using System;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class TurnedOver : MonoBehaviour
    {
        public Action OnTrigger { get; set; }
        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger == false)
                OnTrigger?.Invoke();
        }
    }
}