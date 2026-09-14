using Assets.Scripts.Data;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static Assets.Scripts.Data.ChunkData;

namespace Assets.Scripts.Editor
{
    [CustomEditor(typeof(ChunkData))]
    public class ChunkDataEditor : UnityEditor.Editor
    {
        private string _path = "Assets/Prefabs/Chunks";
        private Material _terrainMaterial;
        private bool _running;
        private float _progress;
        public override void OnInspectorGUI()
        {
            ChunkData data = (ChunkData)target;
            _path = EditorGUILayout.TextField("path", _path);
            _terrainMaterial = (Material)EditorGUILayout.ObjectField("Terrain Material", _terrainMaterial, typeof(Material), false);

            EditorGUILayout.Separator(); // Add space

            if (_running == false)
            {
                if (GUILayout.Button("Cache"))
                {
                    Run(data);
                }
            }
            else
            {
                EditorGUILayout.HelpBox($"click to progress [{_progress:P1}...]", MessageType.Error);
            }
            DrawDefaultInspector();
        }
        private void Run(ChunkData data)
        {
            if (_running)
                return;

            _running = true;
            Cache(data).ContinueWith(() => _running = false);
        }
        private async UniTask Cache(ChunkData target)
        {
            string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { _path });

            List<GameObject> assets = new List<GameObject>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject mat = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (mat != null)
                    assets.Add(mat);
            }

            List<AssetReferenceGameObject> chunks = new();
            foreach (GameObject item in assets)
            {
                string path = AssetDatabase.GetAssetPath(item);
                string guid = AssetDatabase.AssetPathToGUID(path);

                AssetReferenceGameObject assetRef = new AssetReferenceGameObject(guid);
                chunks.Add(assetRef);
            }

            HashSet<AsyncOperationHandle> handles = new();
            List<ChunkConfig> chunkConfigs = new();
            try
            {
                for (int i = 0; i < chunks.Count; i++)
                {
                    AssetReferenceGameObject reference = chunks[i];
                    AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(reference);
                    _progress = (float)i / chunks.Count;
                    handles.Add(handle);
                    GameObject prefab = await handle;

                    Vector2Int pos;
                    if (prefab.TryGetComponent(out Terrain terrain))
                    {
                        Vector3 terrainPos = prefab.transform.position + terrain.terrainData.size / 2;
                        pos = new((int)terrainPos.x, (int)terrainPos.z);
                        if (_terrainMaterial != null)
                        {
                            terrain.materialTemplate = _terrainMaterial;
                            EditorUtility.SetDirty(terrain);
                        }
                    }
                    else
                    {
                        pos = new((int)prefab.transform.position.x, (int)prefab.transform.position.z);
                        pos += new Vector2Int(target.Size / 2, target.Size / 2);
                    }

                    ChunkConfig cfg = new ChunkConfig(pos, reference);
                    chunkConfigs.Add(cfg);
                }

                foreach (AsyncOperationHandle handle in handles)
                {
                    if (handle.IsValid())
                        Addressables.Release(handle);
                }
                target.SetConfigs(chunkConfigs);
                EditorUtility.SetDirty(target);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
