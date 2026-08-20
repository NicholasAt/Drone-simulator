using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Services
{
    public class SceneLoader
    {
        public async UniTask LoadSingle(string key)
        {
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(key, LoadSceneMode.Single);

            await handle.ToUniTask(progress: Progress.Create<float>(value =>
               {
                   //update progress
               }));

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"cant load");
                if (handle.IsValid())
                    Addressables.Release(handle);
            }
        }
    }
}