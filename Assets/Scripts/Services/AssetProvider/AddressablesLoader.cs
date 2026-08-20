using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets.Scripts.Services.AssetProvider
{
    public class AddressablesLoader : IAssetProviderService
    {
        private readonly Dictionary<object, AsyncOperationHandle> _handles = new();      

        public async UniTask<T> LoadAsync<T>(AssetReference reference, CancellationToken ct = default)
        {
            if (reference == null)
            {
                Debug.LogError("zero reference");
                throw new ArgumentNullException(nameof(reference));
            }

            object key = reference.RuntimeKey;

            if (_handles.TryGetValue(key, out AsyncOperationHandle existingHandle) && existingHandle.IsValid())
            {
                AsyncOperationHandle<T> typedHandle = existingHandle.Convert<T>();
                return await typedHandle.ToUniTask(cancellationToken: ct);
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(reference);
            _handles[key] = handle;

            try
            {
                T asset = await handle.ToUniTask(cancellationToken: ct);
                return asset;
            }
            catch (Exception)
            {
                Debug.LogError("cant load asset");
                _handles.Remove(key);
                if (handle.IsValid())
                    Addressables.Release(handle);
                throw;
            }
        }

        public void Release(AssetReference reference)
        {
            if (reference == null)
            {
                Debug.LogError("zero reference");
                throw new ArgumentNullException(nameof(reference));
            }

            object key = reference.RuntimeKey;
            if (_handles.TryGetValue(key, out AsyncOperationHandle handle) && handle.IsValid())
            {
                Addressables.Release(handle);
                _handles.Remove(key);
            }
        }

        public void ReleaseAll()
        {
            foreach (AsyncOperationHandle handle in _handles.Values)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }

            _handles.Clear();
        }
    }
}