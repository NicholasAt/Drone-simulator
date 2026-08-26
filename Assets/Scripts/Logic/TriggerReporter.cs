using System;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class TriggerReporter : MonoBehaviour
    {
        public Action<GameObject> OnTrigger { get; set; }
        private void OnTriggerEnter(Collider other)
        {
            OnTrigger?.Invoke(other.gameObject);
        }
    }
}