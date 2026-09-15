using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(menuName = "Data/Chunks")]
    public class ChunkData : ScriptableObject
    {
        [Serializable]
        public class ChunkConfig
        {
            [field: SerializeField] public Vector2Int Key { get; private set; }
            [field: SerializeField] public AssetReferenceGameObject ChunkReference { get; private set; }
            public ChunkConfig(Vector2Int key, AssetReferenceGameObject chunkReference)
            {
                Key = key;
                ChunkReference = chunkReference;
            }
        }

        [SerializeField] private List<ChunkConfig> _chunkConfigs;
        [SerializeField] private List<AssetReferenceGameObject> _bases;
        public IList<AssetReferenceGameObject> Bases => _bases;
        private Dictionary<Vector2Int, ChunkConfig> _cachedConfigs;
        [field: SerializeField] public AssetReferenceGameObject OutsideMapReference { get; private set; }
        [field: SerializeField] public int ChunkCount { get; private set; } = 3;
        [field: SerializeField] public int Size { get; private set; } = 626;
        [field: SerializeField] public int LoadDistance { get; private set; } = 900;
        [field: SerializeField] public int UnloadDistance { get; private set; } = 950;

        public void Init()
        {
            _cachedConfigs = _chunkConfigs.ToDictionary(x => x.Key, x => x);
        }

        public bool TryConfig(Vector2Int key, out ChunkConfig cfg)
        {
            return _cachedConfigs.TryGetValue(key, out cfg);
        }
        public void SetConfigs(List<ChunkConfig> chunkConfigs)
        {
            _chunkConfigs = chunkConfigs;
        }
    }
}