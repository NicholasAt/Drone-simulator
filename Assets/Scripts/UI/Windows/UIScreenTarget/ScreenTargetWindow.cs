using Assets.Scripts.Data;
using Assets.Scripts.Services;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.UI.Windows.UIScreenTarget
{
    public class ScreenTargetWindow : MonoBehaviour
    {
        [SerializeField] private Pool _pool;
        private OutScreenData _outScreenData;
        private OffScreenContainer _offScreenContainer;
        private GameObserver _gameObserver;
        private Camera _mainCamera;
        private readonly List<UIScreenTargetPoint> _currentPoints = new();

        [Inject]
        private void Construct(OutScreenData outScreenData, OffScreenContainer offScreenContainer, GameObserver gameObserver)
        {
            _outScreenData = outScreenData;
            _offScreenContainer = offScreenContainer;
            _gameObserver = gameObserver;
        }
        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            CheckPoints();
            UpdatePoints();
        }

        private void CheckPoints()
        {
            int difference = _currentPoints.Count - _offScreenContainer.Targets.Count;
            if (difference > 0)
            {
                for (int i = 0; i < difference; i++)
                {
                    if (_currentPoints.Count < difference)
                        continue;

                    UIScreenTargetPoint point = _currentPoints[_currentPoints.Count - 1 - i];
                    _pool.Enqueue(point);
                    _currentPoints.Remove(point);
                }
            }
            else if (difference < 0)
            {
                for (int i = 0; i < Mathf.Abs(difference); i++)
                {
                    UIScreenTargetPoint point = _pool.Dequeue();
                    _currentPoints.Add(point);
                }
            }
        }
        private void UpdatePoints()
        {
            int i = 0;
            foreach (Transform target in _offScreenContainer.Targets)
            {
                Vector3 screenPos = _mainCamera.WorldToScreenPoint(target.position);
                UIScreenTargetPoint indicator = _currentPoints[i];
                float distance = Vector3.Distance(_mainCamera.transform.position, target.position);

                bool isBehind = screenPos.z < 0;

                if (isBehind)
                {
                    screenPos.x = Screen.width - screenPos.x;
                    screenPos.y = Screen.height - screenPos.y;
                }

                bool isOffScreen = isBehind ||
                                   screenPos.x < _outScreenData.EdgeOffset || screenPos.x > Screen.width - _outScreenData.EdgeOffset ||
                                   screenPos.y < _outScreenData.EdgeOffset || screenPos.y > Screen.height - _outScreenData.EdgeOffset;

                if (isOffScreen)
                {
                    Vector2 screenCenter = new Vector2(Screen.width, Screen.height) * 0.5f;
                    Vector2 dir = ((Vector2)screenPos - screenCenter).normalized;

                    float angle = Mathf.Atan2(dir.y, dir.x);
                    float slope = Mathf.Tan(angle);

                    Vector2 edgePos = screenCenter;

                    if (dir.x > 0)
                        edgePos.x = Screen.width - _outScreenData.EdgeOffset;
                    else
                        edgePos.x = _outScreenData.EdgeOffset;

                    edgePos.y = screenCenter.y + slope * (edgePos.x - screenCenter.x);

                    if (edgePos.y > Screen.height - _outScreenData.EdgeOffset)
                    {
                        edgePos.y = Screen.height - _outScreenData.EdgeOffset;
                        edgePos.x = screenCenter.x + (edgePos.y - screenCenter.y) / slope;
                    }
                    else if (edgePos.y < _outScreenData.EdgeOffset)
                    {
                        edgePos.y = _outScreenData.EdgeOffset;
                        edgePos.x = screenCenter.x + (edgePos.y - screenCenter.y) / slope;
                    }
                    indicator.Show(edgePos, dir);
                    indicator.UpdateDistance(distance);
                }
                else
                {
                    if (distance < _outScreenData.RendererDistance)
                    {
                        indicator.Hide();
                    }
                    else
                    {
                        screenPos.x = Mathf.Clamp(screenPos.x, _outScreenData.EdgeOffset, Screen.width - _outScreenData.EdgeOffset);
                        screenPos.y = Mathf.Clamp(screenPos.y, _outScreenData.EdgeOffset, Screen.height - _outScreenData.EdgeOffset);

                        indicator.Show(screenPos);
                        indicator.UpdateDistance(distance);
                    }
                }
                i++;
            }
        }
        [Serializable]
        private class Pool
        {
            [SerializeField] private UIScreenTargetPoint _template;

            private readonly Stack<UIScreenTargetPoint> _pool = new();
            private readonly HashSet<UIScreenTargetPoint> _pooledObjects = new();

            public UIScreenTargetPoint Dequeue()
            {
                if (_pool.Count == 0)
                {
                    return UnityEngine.Object.Instantiate(_template, _template.transform.parent);
                }

                UIScreenTargetPoint point = _pool.Pop();
                _pooledObjects.Remove(point);

                return point;
            }

            public void Enqueue(UIScreenTargetPoint point)
            {
                if (_pooledObjects.Add(point) == false)
                {
                    Debug.LogWarning("Object is already in the pool!");
                    return;
                }

                point.Hide();
                _pool.Push(point);
            }
        }
    }
}