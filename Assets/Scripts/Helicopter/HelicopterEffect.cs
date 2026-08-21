using UnityEngine;

namespace Assets.Scripts.Helicopter
{
    public class HelicopterEffect : MonoBehaviour
    {
        [SerializeField] private HelicopterInput _helicopterController;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Transform _mainRotor, _tailRotor;

        private float _currentCollective;
        private HelicopterInput.HelicopterConfig _config;

        private void Start()
        {
            _config = _helicopterController.Config;
        }
        private void Update()
        {
            UpdateRotors();
            UpdateAudio();
        }

        private void UpdateRotors()
        {
            float collectiveTarget = CollectiveTarget();
            _currentCollective = Mathf.MoveTowards(_currentCollective, collectiveTarget, _config.EffectCollectiveSmoothSpeed * Time.deltaTime);
            float collective = _currentCollective * Time.deltaTime;

            _mainRotor.Rotate(0, collective, 0);
            _tailRotor.Rotate(0, collective, 0);
        }

        private void UpdateAudio()
        {
            float collectiveNormalized = _currentCollective / _config.EffectSpeedRotate.y;
            float pith = _config.AudioPitchCurve.Evaluate(collectiveNormalized);
            if (pith < 0.2)//fix broken sound
                pith = 0;
            _audioSource.pitch = pith;
        }

        private float CollectiveTarget()
        {
            return _helicopterController.IsEngineEnable == false ? _helicopterController.CurrentCollective
                : _helicopterController.IsLift ? _config.EffectSpeedRotate.y
                : _helicopterController.IsLowering ? _config.EffectSpeedRotate.x
                : (_config.EffectSpeedRotate.x + _config.EffectSpeedRotate.y) / 2;
        }
    }
}