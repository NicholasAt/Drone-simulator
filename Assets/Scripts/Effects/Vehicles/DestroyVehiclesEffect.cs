using Assets.Scripts.Data.DestroyVehiclesEffect;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Effects.Vehicles
{
    public class DestroyVehiclesEffect : MonoBehaviour, IVehiclesDestroyEffect
    {
        [SerializeField] private ParticleSystem[] _particleSystem;
        private AudioSource _audioSource;
        private VehiclesDestroyEffectData _effectData;
        private VehiclesDestroyEffectConfig _config;
        private readonly List<(Rigidbody body, Vector3 initPosition)> _parts = new();

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
            foreach (Rigidbody body in GetComponentsInChildren<Rigidbody>())
            {
                _parts.Add((body, body.transform.localPosition));
                if (body.TryGetComponent(out Collider collider))
                {
                    foreach ((Rigidbody, Vector3) bodyTarget in _parts)
                    {
                        if (bodyTarget.Item1.TryGetComponent(out Collider colliderTarget))
                            Physics.IgnoreCollision(collider, colliderTarget);
                    }
                }
            }
            _audioSource = GetComponent<AudioSource>();
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
            gameObject.SetActive(true);
            transform.position = pos;
            transform.up = direction.normalized;
            Vector3 upPos = transform.position + transform.up * _config.ConeHeight;
            for (int i = 0; i < _parts.Count; i++)
            {
                (Rigidbody body, Vector3 initPosition) = _parts[i];
                Rigidbody rb = body;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.transform.localPosition = initPosition;

                Vector2 circlePos = UnityEngine.Random.insideUnitCircle * _config.CircleRadius;
                Vector3 boomPos = upPos + new Vector3(circlePos.x, 0, circlePos.y);
                Vector3 boomDirection = boomPos - transform.position;

                rb.AddForce(boomDirection.normalized * _config.Force, ForceMode.Impulse);
                rb.AddTorque(UnityEngine.Random.insideUnitSphere * _config.Torque, ForceMode.Impulse);
            }
            PlayEffect();
            PlayAudio();
            Hide(_effectData.LifeSeconds).Forget();
        }
        private void PlayEffect()
        {
            foreach (ParticleSystem effect in _particleSystem)
            {
                effect.Stop();
                effect.Play();
            }
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