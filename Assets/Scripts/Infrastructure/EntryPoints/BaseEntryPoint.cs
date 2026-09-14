using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Infrastructure.EntryPoints
{
    public abstract class BaseEntryPoint : MonoBehaviour
    {
        protected CancellationToken CancelToken;
        private async UniTaskVoid Start()
        {
            CancelToken = this.GetCancellationTokenOnDestroy();
            await OnStart();
        }
        protected virtual async UniTask OnStart()
        {
            await UniTask.CompletedTask;
        }
    }
}