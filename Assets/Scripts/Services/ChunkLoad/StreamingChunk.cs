using Assets.Scripts.Data;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets.Scripts.Services.ChunkLoad
{
    public class StreamingChunk
    {
        private const int MaxTreeDistance = 500;
        private const int MinTreeDistance = 0;

        public readonly Dictionary<Vector2Int, AsyncOperationHandle> Active = new();
        private readonly HashSet<Vector2Int> _candidatesForUnloading = new();
        private readonly ChunkData _chunkData;

        public StreamingChunk(ChunkData chunkData)
        {
            _chunkData = chunkData;
        }
        public async UniTask<bool> TryLoad(Vector2Int key, CancellationToken ct)
        {
            if (_chunkData.TryConfig(key, out ChunkData.ChunkConfig cfg))
            {
                if (Active.ContainsKey(key) == false)
                {
                    try
                    {
                        AsyncOperationHandle<GameObject> handler = Addressables.InstantiateAsync(cfg.ChunkReference);
                        Active.Add(key, handler);
                        GameObject instance = await handler.ToUniTask(cancellationToken: ct);
                        if (instance.TryGetComponent(out Terrain terrain))
                        {
                            if (BrowserDetector.IsSafariBrowser())
                            {
                                terrain.treeDistance = MinTreeDistance;
                            }
                            else
                            {
                                terrain.treeDistance = MaxTreeDistance;
                            }
                        }
                        return true;
                    }
                    catch (OperationCanceledException)
                    {
                        return false;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        return false;
                    }
                }
            }
            return false;
        }

        public void AddUnloadKey(Vector2Int key)
        {
            _candidatesForUnloading.Add(key);
        }
        public void Unload()
        {
            foreach (Vector2Int key in _candidatesForUnloading)
            {
                if (Active.TryGetValue(key, out AsyncOperationHandle handle) == false)
                {
                    Debug.LogError("no key");
                    return;
                }

                if (handle.IsValid())
                    Addressables.ReleaseInstance(handle);
                Active.Remove(key);
            }
            _candidatesForUnloading.Clear();
        }
        public void ClearAll()
        {
            foreach (AsyncOperationHandle item in Active.Values)
            {
                if (item.IsValid())
                    Addressables.ReleaseInstance(item);
            }
            Active.Clear();
            _candidatesForUnloading.Clear();
        }
    }
}