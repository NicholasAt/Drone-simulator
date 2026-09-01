using Assets.Scripts.Data.CameraAnimationData;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Services.CameraService
{

    public class CameraStateService
    {
        private class CameraAnimation
        {
            private const int AngleStep = 15;
            private const float CastSize = 1;

            private Transform _cameraTransform;
            private float _height;
            private float _width;
            private float _duration;
            private LayerMask _ignoreMask;
            private readonly List<int> _angles = new();
            private readonly Collider[] _colliders = new Collider[150];
            private readonly GameObserver _gameObserver;

            public CameraAnimation(GameObserver gameObserver)
            {
                _gameObserver = gameObserver;
            }
            public void Prepare(Transform cameraTransform, float height, float width, float duration, LayerMask ignoreMask)
            {
                _cameraTransform = cameraTransform;
                _height = height;
                _width = width;
                _duration = duration;
                _ignoreMask = ignoreMask;
            }

            public async UniTask Play(Vector3 from, Action onEnd, CancellationToken ct)
            {
                try
                {
                    Vector3 pos = Vector3.zero;
                    _angles.Clear();
                    for (int i = 0; i < 360; i += AngleStep)
                    {
                        _angles.Add(i);
                    }

                    int stack = _angles.Count;
                    while (true)
                    {
                        stack--;
                        if (stack < 0)
                        {
                            Debug.LogError("no pos");
                            break;
                        }
                        if (_angles.Count <= 0)
                            continue;

                        int index = UnityEngine.Random.Range(0, _angles.Count);
                        int angle = _angles[index];
                        _angles.RemoveAt(index);

                        float randomRad = angle * Mathf.Deg2Rad;
                        Vector2 circle = new Vector2(Mathf.Cos(randomRad), Mathf.Sin(randomRad)) * _width;
                        pos = from + new Vector3(circle.x, _height, circle.y);
                        int collidersCount = Physics.OverlapSphereNonAlloc(pos, CastSize, _colliders, ~_ignoreMask, QueryTriggerInteraction.Ignore);

                        if (collidersCount > 0)
                        {
                            continue;
                        }

                        if (Physics.SphereCast(pos, CastSize, (from - pos).normalized, out RaycastHit hit, Vector3.Distance(from, pos) - 1.5f, ~_ignoreMask, QueryTriggerInteraction.Ignore) == false)
                            break;
                    }

                    _cameraTransform.parent = null;
                    _cameraTransform.position = pos;
                    _cameraTransform.LookAt(from);

                    await UniTask.WaitForSeconds(_duration, cancellationToken: ct);
                    await UniTask.WaitUntil(() => _gameObserver.IsPause == false, cancellationToken: ct);

                    onEnd?.Invoke();
                }
                catch (System.OperationCanceledException)
                {
                    //ignore
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        private Camera _mainCamera;
        private readonly CameraAnimation _animation;
        private readonly CameraData _cameraData;

        public CameraStateService(CameraData cameraData, GameObserver gameObserver)
        {
            _cameraData = cameraData;
            _animation = new(gameObserver);
        }
        public void Prepare()
        {
            _mainCamera = Camera.main;
            _animation.Prepare(_mainCamera.transform, _cameraData.Height, _cameraData.Width, _cameraData.Duration, _cameraData.IgnoreMask);
        }

        public async UniTask Show(Vector3 from, Action onEnd, CancellationToken ct)
        {
            await _animation.Play(from, onEnd, ct);
        }

        public void SetParent(Transform parent)
        {
            _mainCamera.transform.SetParent(parent);
            _mainCamera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}