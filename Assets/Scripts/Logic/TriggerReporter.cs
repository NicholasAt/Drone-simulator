using System;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class TriggerReporter : MonoBehaviour
    {
        public Action<GameObject> OnTrigger { get; set; }
        [SerializeField] private bool _reportTrigger = true;
        private void OnTriggerEnter(Collider other)
        {
            if (_reportTrigger == false)
            {
                if (other.isTrigger || other.gameObject.activeInHierarchy == false)
                    return;
            }
            OnTrigger?.Invoke(other.gameObject);
        }
    }
}