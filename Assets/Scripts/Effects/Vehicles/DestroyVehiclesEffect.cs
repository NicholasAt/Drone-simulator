using Assets.Scripts.Data.DestroyVehiclesEffect;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Effects.Vehicles
{
    public class DestroyVehiclesEffect : MonoBehaviour, IVehiclesDestroyEffect
    {
        private Rigidbody[] _parts;


        private VehiclesDestroyEffectData _effectData;
        private VehiclesDestroyEffectConfig _config;

        [Inject]
        private void Construct(VehiclesDestroyEffectData data)
        {
            _effectData = data;
        }
        private void Awake()
        {
            _parts = GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody body in _parts)
            {
                if (body.TryGetComponent(out Collider collider))
                {
                    foreach (Rigidbody bodyTarget in _parts)
                    {
                        if (bodyTarget.TryGetComponent(out Collider colliderTarget))
                            Physics.IgnoreCollision(collider, colliderTarget);
                    }
                }
            }
        }

        public void Init(DestroyEffectId id)
        {
            _config = _effectData.GetConfig(id);
        }
        public void Run(Vector3 pos, Vector3 direction)
        {
            if (_parts == null || _parts.Length == 0)
                Debug.LogError("no parts");

            transform.position = pos;
            transform.up = direction.normalized;
            Vector3 upPos = transform.position + transform.up * _config.ConeHeight;
            for (int i = 0; i < _parts.Length; i++)
            {
                Rigidbody rb = _parts[i];
                Vector2 circlePos = Random.insideUnitCircle * _config.CircleRadius;
                Vector3 boomPos = upPos + new Vector3(circlePos.x, 0, circlePos.y);
                Vector3 boomDirection = boomPos - transform.position;

                rb.AddForce(boomDirection.normalized * _config.Force, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * _config.Torque, ForceMode.Impulse);
            }
        }
    }

    public interface IVehiclesDestroyEffect
    {
        void Init(DestroyEffectId id);
        void Run(Vector3 pos, Vector3 direction);
    }
}