using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class OffScreenContainer
    {
        private readonly HashSet<Transform> _targets = new();
        public ICollection<Transform> Targets => _targets;
        public void Cleanup()
        {
            _targets.Clear();
        }

        public void Add(Transform target)
        {
            _targets.Add(target);
        }

        public void Remove(Transform target)
        {
            _targets.Remove(target);
        }
    }
}