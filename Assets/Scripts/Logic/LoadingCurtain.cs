using Assets.Scripts.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class LoadingCurtain : MonoBehaviour
    {
        [SerializeField] private WindowAnimation _windowAnimation;
        public async UniTask Show()
        {
            await _windowAnimation.Show();
        }

        public async UniTask Hide()
        {
            await _windowAnimation.Hide();
        }
    }
}