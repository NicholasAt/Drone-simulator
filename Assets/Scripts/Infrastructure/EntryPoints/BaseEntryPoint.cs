using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Infrastructure.EntryPoints
{
    public abstract class BaseEntryPoint : MonoBehaviour
    {
        private async UniTaskVoid Start()
        {
            await OnStart();
        }
        protected virtual async UniTask OnStart()
        {
            await UniTask.CompletedTask;
        }
    }
}