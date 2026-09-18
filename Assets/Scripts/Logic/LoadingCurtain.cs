using Assets.Scripts.UI;
using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class LoadingCurtain : MonoBehaviour
    {
        [SerializeField] private WindowAnimation _windowAnimation;
        [SerializeField] private TMP_Text _progressText;
        public async UniTask Show()
        {
            await _windowAnimation.Show();
        }

        public async UniTask Hide()
        {
            await _windowAnimation.Hide();
        }

        public void UpdateProgress(float progress)
        {
            _progressText.text = $"Loading {progress:P0}";
        }
    }
}