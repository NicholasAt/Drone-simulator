using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class LoadingCurtain : MonoBehaviour
    {
        public async UniTask Show()
        {
            gameObject.SetActive(true);
            await UniTask.NextFrame();
        }

        public async UniTask Hide()
        {
            gameObject.SetActive(false);
            await UniTask.CompletedTask;
        }
    }
}