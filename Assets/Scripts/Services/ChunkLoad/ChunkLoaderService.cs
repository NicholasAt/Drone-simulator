using Assets.Scripts.Data;
using Assets.Scripts.Services.AssetProvider;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Services.ChunkLoad
{
    public class ChunkLoaderService
    {
        private Transform _target;

        private Vector2Int _cell;
        private Vector2Int _chunkKey;
        private CancellationTokenSource _cts;
        private readonly ChunkData _chunkData;
        private readonly IAssetProviderService _assetProvider;
        private readonly StreamingChunk _streamingChunk;
        private bool _isUnload = true;
        public ChunkLoaderService(StreamingChunk streamingChunk, ChunkData chunkData, IAssetProviderService assetProviderService)
        {
            _streamingChunk = streamingChunk;
            _chunkData = chunkData;
            _assetProvider = assetProviderService;
        }
        public void SetUnload(bool isUnload)
        {
            _isUnload = isUnload;
        }
        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public async UniTask Run()
        {
            if (_cts != null)
            {
                Debug.LogWarning("already working");
                return;
            }

            _cts = new CancellationTokenSource();
            await UpdateLoad(GetChunkKey(), _cts.Token);
            CheckChunkTimer(0.5f, _cts.Token).Forget();
        }
        public void Stop()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
            _isUnload = true;
            _streamingChunk.ClearAll();
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

            if (_isUnload)
                Unload();

            await UniTask.NextFrame(cancellationToken: ct);
            int min = -Mathf.FloorToInt(_chunkData.ChunkCount / 2f);
            for (int i = 0; i < _chunkData.ChunkCount; i++)
            {
                int yKey = (min + i) * _chunkData.Size;
                for (int j = 0; j < _chunkData.ChunkCount; j++)
                {
                    int xKey = (min + j) * _chunkData.Size;
                    Vector2Int key = rootKey + new Vector2Int(xKey, yKey);

                    if (key == rootKey)//skip main
                        continue;

                    if (_target == null)
                        break;
                    float distance = Vector2.Distance(key, new Vector2(_target.position.x, _target.position.z));
                    if (distance > _chunkData.LoadDistance)
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
                float distance = Vector2.Distance(new Vector2(_target.position.x, _target.position.z), key);
                if (distance > _chunkData.UnloadDistance)
                    _streamingChunk.AddUnloadKey(key);
            }
            _streamingChunk.Unload();
        }
        private Vector2Int GetChunkKey()
        {
            int halfSize = _chunkData.Size / 2;
            Vector2Int pos = new(Mathf.FloorToInt(_target.position.x / _chunkData.Size) * _chunkData.Size, Mathf.FloorToInt(_target.position.z / _chunkData.Size) * _chunkData.Size);
            _cell = new Vector2Int(pos.x / _chunkData.Size, pos.y / _chunkData.Size);
            _chunkKey = pos + new Vector2Int(halfSize, halfSize);
            return _chunkKey;
        }
    }
}