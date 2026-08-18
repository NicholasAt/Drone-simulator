using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    public class SetTreeEditor : EditorWindow
    {
        private Vector2Int _newTreeCount = new Vector2Int(1, 5);
        private Vector2 _scale = new Vector2(0.7f, 1.3f);

        private float _radius = 6;
        private GameObject _treePrefab;
        private float _chunkSize = 500;
        private Transform _treeRoot, _chunksRoot;

        [MenuItem("Tools/Tree tool")]
        private static void CreatWindow()
        {
            GetWindow<SetTreeEditor>(false, "Tree tool", true);
        }

        private void OnGUI()
        {
            _newTreeCount = EditorGUILayout.Vector2IntField("New tree count", _newTreeCount);
            _scale = EditorGUILayout.Vector2Field("Scale", _scale);
            _radius = EditorGUILayout.FloatField("Radius", _radius);
            _chunkSize = EditorGUILayout.FloatField("Chunk size", _chunkSize);

            _chunksRoot = (Transform)EditorGUILayout.ObjectField(new GUIContent("Chunks root"), _chunksRoot, typeof(Transform), true);
            _treeRoot = (Transform)EditorGUILayout.ObjectField(new GUIContent("Tree root"), _treeRoot, typeof(Transform), true);
            _treePrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Tree prefab"), _treePrefab, typeof(GameObject), true);

            if (GUILayout.Button("Add tree group"))
            {
                AddTreeGroup();
            }
            if (GUILayout.Button("Set cells"))
            {
                SetCells();
            }
            if (GUILayout.Button("Correct Y"))
            {
                CorrectY();
            }
        }
        private void CorrectY()
        {
            if (_treeRoot != null)
            {
                List<Collider> colliders = _treeRoot.GetComponentsInChildren<Collider>().ToList();
                colliders.ForEach(x => x.enabled = false);
                for (int i = 0; i < _treeRoot.childCount; i++)
                {
                    GameObject item = _treeRoot.GetChild(i).gameObject;
                    Vector3 pos = item.transform.position;
                    if (Physics.Raycast(pos + Vector3.up * 10_000, Vector3.down, out RaycastHit hit))
                    {
                        item.transform.position = hit.point;
                        EditorUtility.SetDirty(item);
                    }
                    else
                        Debug.LogError("cant");
                }
                colliders.ForEach(x => x.enabled = true);
            }
            else
                Debug.Log("no root");
        }
        private void AddTreeGroup()
        {
            if (_treePrefab == null)
            {
                Debug.LogError("no prefab");
                return;
            }
            if (_treeRoot != null)
            {
                List<GameObject> newTrees = new();
                for (int i = 0; i < _treeRoot.childCount; i++)
                {
                    GameObject item = _treeRoot.GetChild(i).gameObject;
                    item.transform.localScale = RandomScale();

                    item.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    EditorUtility.SetDirty(item);

                    int count = Random.Range(_newTreeCount.x, _newTreeCount.y);
                    for (int j = 0; j < count; j++)
                    {
                        Vector2 raduus = (Random.insideUnitCircle + Vector2.one) * _radius;
                        Vector3 pos = item.transform.position + new Vector3(raduus.x, 0, raduus.y);

                        if (Physics.Raycast(pos + Vector3.up * 10_000, Vector3.down, out RaycastHit hit))
                        {
                            GameObject instance = Instantiate(_treePrefab, hit.point, Quaternion.Euler(0, Random.Range(0, 360), 0));
                            instance.transform.localScale = RandomScale();
                            newTrees.Add(instance);
                            EditorUtility.SetDirty(instance);
                        }
                        else
                            Debug.LogError("cant");
                    }
                }
                foreach (GameObject tree in newTrees)
                {
                    tree.transform.SetParent(_treeRoot);
                }
            }
        }
        private void SetCells()
        {
            if (_treeRoot == null || _chunksRoot == null)
                return;

            for (int i = 0; i < _chunksRoot.childCount; i++)
            {
                GameObject chunk = _chunksRoot.GetChild(i).gameObject;

                List<GameObject> treesCandidates = new();
                float halfSize = _chunkSize / 2;
                Bounds chunkBounds = new Bounds()
                {
                    center = chunk.transform.position + new Vector3(halfSize, 0, halfSize),
                    extents = new Vector3(halfSize, 1000, halfSize)
                };
                for (int j = 0; j < _treeRoot.childCount; j++)
                {
                    GameObject tree = _treeRoot.GetChild(j).gameObject;
                    if (chunkBounds.Contains(tree.transform.position))
                    {
                        treesCandidates.Add(tree);
                    }
                }
                if (treesCandidates.Count > 0)
                {
                    Transform treeRoot = new GameObject("Trees").transform;
                    treeRoot.SetParent(chunk.transform);
                    foreach (GameObject tree in treesCandidates)
                    {
                        tree.transform.SetParent(treeRoot);
                        EditorUtility.SetDirty(tree);
                    }
                    EditorUtility.SetDirty(chunk);
                }
            }
        }

        private Vector3 RandomScale() =>
            Vector3.one * Random.Range(_scale.x, _scale.y);
    }
}