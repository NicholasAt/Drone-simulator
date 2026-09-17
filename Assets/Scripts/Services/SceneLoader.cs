using Assets.Scripts.Data;
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
        private readonly UIData _uIData;
        private AsyncOperationHandle<GameObject> _curtainHandle;
        private LoadingCurtain _curtain;

        public SceneLoader(UIData uIData)
        {
            _uIData = uIData;
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
            _curtainHandle = Addressables.LoadAssetAsync<GameObject>(_uIData.LoadingCurtainReference);
            GameObject prefab = await _curtainHandle.ToUniTask();
            GameObject instance = Object.Instantiate(prefab);
            Object.DontDestroyOnLoad(instance);
            if (instance.TryGetComponent(out LoadingCurtain curtain) == false)
                Debug.LogError("no component");

            _curtain = curtain;
        }
    }
}