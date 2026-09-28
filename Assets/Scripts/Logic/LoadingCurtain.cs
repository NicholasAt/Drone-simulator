using Assets.Scripts.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class LoadingCurtain : MonoBehaviour
    {
        [SerializeField] private WindowAnimation _windowAnimation;
        [SerializeField] private TMP_Text _progressText;
        private float _previousWeight;
        private float _currentWeight;

        public async UniTask Show()
        {
            Clear();
            await _windowAnimation.Show();
        }

        public async UniTask Hide()
        {
            await _windowAnimation.Hide();
        }

        public void UpdateProgress(float current, float weight)
        {
            if (_currentWeight != weight)
            {
                _previousWeight = _currentWeight;
                _currentWeight = weight;
            }

            float result = Mathf.Lerp(_previousWeight, _currentWeight, current);
            string texts = $"Loading {result:P0}";
            _progressText.text = texts;
        }
        private void Clear()
        {
            _previousWeight = 0;
            _currentWeight = 0;
            _progressText.text = $"Loading {0:P0}"; ;
        }
    }
}