using Assets.Scripts.Logic;
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
        private readonly UIFactory _uIFactory;
        private LoadingCurtain _curtain;

        public SceneLoader(UIFactory uIFactory)
        {
            _uIFactory = uIFactory;
        }
        public async UniTask Init()
        {
            await CreateCurtain();
        }

        public async UniTask LoadSingle(string key)
        {
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(key, LoadSceneMode.Single);
            await handle.ToUniTask();

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"cant load");
                if (handle.IsValid())
                    Addressables.Release(handle);
            }
        }
        public async UniTask ShowCurtain()
        {
            _curtain.UpdateProgress(0);
            await _curtain.Show();
        }
        public async UniTask HideCurtain()
        {
            await _curtain.Hide();
        }
        public void UpdateProgress(float progress)
        {
            _curtain.UpdateProgress(progress);
        }
        private async UniTask CreateCurtain()
        {
            _curtain = await _uIFactory.CreateLoadingCurtain();
            Object.DontDestroyOnLoad(_curtain);
        }
    }
}