using Assets.Scripts.Data.DestroyVehiclesEffect;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Effects.Vehicles
{
    public class DestroyVehiclesEffect : MonoBehaviour, IVehiclesDestroyEffect
    {
        private AudioSource _audioSource;
        private Rigidbody[] _parts;
        private VehiclesDestroyEffectData _effectData;
        private VehiclesDestroyEffectConfig _config;

        public Action OnHide { get; set; }
        private bool _isProcess;
        private static bool _doublePlayAudio;

        [Inject]
        private void Construct(VehiclesDestroyEffectData data)
        {
            _effectData = data;
        }

        private void Awake()
        {
            _parts = GetComponentsInChildren<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();
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
        private void LateUpdate()
        {
            _doublePlayAudio = false;
        }
        public void Init(DestroyEffectId id)
        {
            _config = _effectData.GetConfig(id);
        }

        public void Show(Vector3 pos, Vector3 direction)
        {
            if (_parts == null || _parts.Length == 0)
                Debug.LogError("no parts");

            gameObject.SetActive(true);
            transform.position = pos;
            transform.up = direction.normalized;
            Vector3 upPos = transform.position + transform.up * _config.ConeHeight;
            for (int i = 0; i < _parts.Length; i++)
            {
                Rigidbody rb = _parts[i];
                Vector2 circlePos = UnityEngine.Random.insideUnitCircle * _config.CircleRadius;
                Vector3 boomPos = upPos + new Vector3(circlePos.x, 0, circlePos.y);
                Vector3 boomDirection = boomPos - transform.position;

                rb.AddForce(boomDirection.normalized * _config.Force, ForceMode.Impulse);
                rb.AddTorque(UnityEngine.Random.insideUnitSphere * _config.Torque, ForceMode.Impulse);
            }

            PlayAudio();
            Hide(_effectData.LifeSeconds).Forget();
        }

        private async UniTaskVoid Hide(float lifeTime)
        {
            try
            {
                if (_isProcess)
                    Debug.LogError("already work");
                _isProcess = true;

                await UniTask.Delay(TimeSpan.FromSeconds(lifeTime), cancellationToken: this.GetCancellationTokenOnDestroy());
                if (_audioSource != null)
                    _audioSource.Stop();
                gameObject.SetActive(false);
                _isProcess = false;
                OnHide?.Invoke();
            }
            catch (OperationCanceledException)
            {
                //ignore
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
        private void PlayAudio()
        {
            if (_doublePlayAudio == false)
            {
                _doublePlayAudio = true;
                if (_audioSource != null)
                    _audioSource.Play();
            }
        }
    }

    public interface IVehiclesDestroyEffect
    {
        void Init(DestroyEffectId id);
        void Show(Vector3 pos, Vector3 direction);
        Action OnHide { get; set; }
    }
}