using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class HitHandler
    {
        private float _radius = 1;
        private Collider[] _colliders = new Collider[100];
        public void Init(float checkRadius, int collidersCount = 100)
        {
            _radius = checkRadius;
            _colliders = new Collider[collidersCount];
        }
        public bool TryDamage(Vector3 pos)
        {
            int count = Physics.OverlapSphereNonAlloc(pos, _radius, _colliders, ~0, QueryTriggerInteraction.Ignore);
            bool wasHit = false;
            for (int i = 0; i < count; i++)
            {
                Transform candidate = _colliders[i].transform.parent;
                if (candidate == null)
                    candidate = _colliders[i].transform;

                if (candidate != null && candidate.TryGetComponent(out IApplyDamage applyDamage))
                {
                    applyDamage.Hit(0);
                    wasHit = true;
                }
            }
            return wasHit;
        }
    }
}