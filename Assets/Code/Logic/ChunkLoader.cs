using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets.Code.Logic
{
    public class ChunkLoader : MonoBehaviour
    {
        public class StreamingChunk
        {
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
                            await handler.ToUniTask(cancellationToken: ct);
                            return true;
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
        }
        [SerializeField] private bool _play = true;
        [SerializeField] private ChunkData _chunkData;
        [SerializeField] private Transform _player;
        [SerializeField] private int _chunkCount = 3;
        [SerializeField] private int _size = 500;
        [SerializeField] private int _loadDistance = 600;
        [SerializeField] private int _unloadDistance = 750;

        private Vector2Int _cell;
        private Vector2Int _chunkKey;
        private StreamingChunk _streamingChunk;

        private async UniTask Start()
        {
            if (_play == false)
                return;

            CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();
            await Addressables.InitializeAsync().ToUniTask(cancellationToken: cancellationToken);

            _chunkData.Init();
            _streamingChunk = new(_chunkData);

            await UpdateLoad(GetChunkKey(), cancellationToken);
            CheckChunkTimer(0.5f, cancellationToken).Forget();
        }

        private async UniTask CheckChunkTimer(float delay, CancellationToken ct)
        {
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: ct);
                await UpdateLoad(GetChunkKey(), ct);
            }
        }

        private async UniTask UpdateLoad(Vector2Int rootKey, CancellationToken ct)
        {
            await _streamingChunk.TryLoad(rootKey, ct);
            Unload();
            await UniTask.NextFrame(cancellationToken: ct);

            int min = -Mathf.FloorToInt(_chunkCount / 2f);
            for (int i = 0; i < _chunkCount; i++)
            {
                int yKey = (min + i) * _size;
                for (int j = 0; j < _chunkCount; j++)
                {
                    int xKey = (min + j) * _size;
                    Vector2Int key = rootKey + new Vector2Int(xKey, yKey);

                    if (key == rootKey)//skip main
                        continue;

                    float distance = Vector2.Distance(key, new Vector2(_player.position.x, _player.position.z));
                    if (distance > _loadDistance)
                        continue;

                    await _streamingChunk.TryLoad(key, ct);
                    await UniTask.NextFrame(cancellationToken: ct);
                }
            }
        }

        private void Unload()
        {
            foreach (Vector2Int key in _streamingChunk.Active.Keys)
            {
                float distance = Vector2.Distance(new Vector2(_player.position.x, _player.position.z), key);
                if (distance > _unloadDistance)
                    _streamingChunk.AddUnloadKey(key);
            }
            _streamingChunk.Unload();
        }
        private Vector2Int GetChunkKey()
        {
            int halfSize = _size / 2;
            Vector2Int pos = new(Mathf.FloorToInt(_player.position.x / _size) * _size, Mathf.FloorToInt(_player.position.z / _size) * _size);
            _cell = new Vector2Int(pos.x / _size, pos.y / _size);
            _chunkKey = pos + new Vector2Int(halfSize, halfSize);
            return _chunkKey;
        }
    }
}