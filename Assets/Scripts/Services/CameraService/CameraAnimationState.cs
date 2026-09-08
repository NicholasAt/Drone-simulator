using Assets.Scripts.Data.CameraAnimationData;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Services.CameraService
{
    public class CameraAnimationState : ICameraEnterParam3<Vector3, Action, CancellationToken>
    {
        private const int AngleStep = 15;
        private const float CastSize = 1;

        private Transform _cameraTransform;
        private GameObject _cinema;
        private float _height;
        private float _width;
        private float _duration;
        private LayerMask _ignoreMask;
        private readonly List<int> _angles = new();
        private readonly Collider[] _colliders = new Collider[100];
        private readonly GameObserver _gameObserver;
        private readonly CameraData _cameraData;
        private readonly GameFactory _gameFactory;

        public CameraAnimationState(GameObserver gameObserver, CameraData cameraData, GameFactory gameFactory)
        {
            _gameObserver = gameObserver;
            _cameraData = cameraData;
            _gameFactory = gameFactory;
        }

        public async UniTask Prepare()
        {
            _cameraTransform = Camera.main.transform;
            _cinema = _gameFactory.CinemaCamera;
            _height = _cameraData.Height;
            _width = _cameraData.Width;
            _duration = _cameraData.Duration;
            _ignoreMask = _cameraData.IgnoreMask;
            await UniTask.CompletedTask;
        }

        public async UniTask Enter(Vector3 from, Action onEnd, CancellationToken ct)
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
                        Debug.LogWarning("no pos");
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

                _cinema.SetActive(false);
                _cameraTransform.parent = null;
                _cameraTransform.position = pos;
                _cameraTransform.LookAt(from);

                float current = 0;
                while (true)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: ct);
                    if (_gameObserver.IsPause)
                        continue;

                    current += Time.deltaTime;
                    if (current >= _duration)
                        break;
                }

                onEnd?.Invoke();
            }
            catch (OperationCanceledException)
            {
                //ignore
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public async UniTask Exit()
        {
            await UniTask.CompletedTask;
        }
    }
}