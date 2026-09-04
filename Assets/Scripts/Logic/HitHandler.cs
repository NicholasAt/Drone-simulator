using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class HitHandler
    {
        private float _radius = 1;
        private Collider[] _colliders = new Collider[100];
        readonly List<GameObject> _hitTargets = new();
        public IList<GameObject> Targets => _hitTargets;
        public void Init(float checkRadius, int collidersCount = 100)
        {
            _radius = checkRadius;
            _colliders = new Collider[collidersCount];
        }

        public bool TryDamage(Vector3 pos)
        {
            _hitTargets.Clear();
            int count = Physics.OverlapSphereNonAlloc(pos, _radius, _colliders, ~0, QueryTriggerInteraction.Ignore);
            bool wasHit = false;
            for (int i = 0; i < count; i++)
            {
                Transform candidate = _colliders[i].transform.parent;
                if (candidate == null)
                    candidate = _colliders[i].transform;

                if (candidate != null && candidate.TryGetComponent(out IApplyDamage applyDamage))
                {
                    wasHit = true;
                    applyDamage.Hit(0);
                    _hitTargets.Add(candidate.gameObject);
                }
            }
            return wasHit;
        }
    }
}