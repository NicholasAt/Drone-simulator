using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

#if UNITY_EDITOR
        [Header("Editor settings")]
        [SerializeField] private float _editorChunkSize = 500;
        [SerializeField] private List<GameObject> _chunksEditor;
#endif
        [Header("Main settings")]
        [SerializeField] private List<ChunkConfig> _chunkConfigs;
        public IList<ChunkConfig> ChunkConfigs => _chunkConfigs;
        private Dictionary<Vector2Int, ChunkConfig> _cachedConfigs;
        public IDictionary<Vector2Int, ChunkConfig> CachedConfigs => _cachedConfigs;

        [field: SerializeField] public int ChunkCount { get; private set; } = 3;
        [field: SerializeField] public int Size { get; private set; } = 500;
        [field: SerializeField] public int LoadDistance { get; private set; } = 900;
        [field: SerializeField] public int UnloadDistance { get; private set; } = 950;

#if UNITY_EDITOR
        [ContextMenu("Cache/Run")]
        private async UniTask Cache()
        {
            List<AssetReferenceGameObject> chunks = new();
            foreach (GameObject item in _chunksEditor)
            {
                string path = UnityEditor.AssetDatabase.GetAssetPath(item);
                string guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);

                AssetReferenceGameObject assetRef = new AssetReferenceGameObject(guid);
                chunks.Add(assetRef);
            }

            HashSet<AsyncOperationHandle> handles = new();
            _chunkConfigs = new();

            foreach (AssetReferenceGameObject reference in chunks)
            {
                AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(reference);

                handles.Add(handle);
                GameObject prefab = await handle;

                Vector2Int pos;
                if (prefab.TryGetComponent(out Terrain terrain))
                {
                    Vector3 terrainPos = prefab.transform.position + terrain.terrainData.size / 2;
                    pos = new((int)terrainPos.x, (int)terrainPos.z);
                }
                else
                {
                    pos = new((int)prefab.transform.position.x, (int)prefab.transform.position.z);
                    pos += new Vector2Int((int)_editorChunkSize / 2, (int)_editorChunkSize / 2);
                }

                ChunkConfig cfg = new ChunkConfig(pos, reference);
                _chunkConfigs.Add(cfg);
            }

            foreach (AsyncOperationHandle handle in handles)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        public void Init()
        {
            _cachedConfigs = _chunkConfigs.ToDictionary(x => x.Key, x => x);
        }

        public bool TryConfig(Vector2Int key, out ChunkConfig cfg)
        {
            return _cachedConfigs.TryGetValue(key, out cfg);
        }
    }
}