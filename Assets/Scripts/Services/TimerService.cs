using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class TimerService
    {
        public Action OnTick { get; set; }

        public int Seconds { get; private set; }
        public bool IsRunning { get; private set; }

        private float _elapsedTime;
        private CancellationTokenSource _cts;
        private readonly GameObserver _gameObserver;

        public TimerService(GameObserver gameObserver)
        {
            _gameObserver = gameObserver;
        }

        public void Start(int startValue = 0)
        {
            if (IsRunning)
            {
                Debug.LogError("Timer is already running");
                return;
            }

            Seconds = startValue;
            _elapsedTime = 0;

            IsRunning = true;

            _cts = new CancellationTokenSource();
            Run(_cts.Token).Forget();
        }

        public void Stop()
        {
            if (!IsRunning)
                return;

            IsRunning = false;

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private async UniTaskVoid Run(CancellationToken ct)
        {
            try
            {
                while (ct.IsCancellationRequested == false)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);

                    if (_gameObserver.IsPause)
                        continue;

                    _elapsedTime += Time.deltaTime;

                    while (_elapsedTime >= 1f)
                    {
                        _elapsedTime -= 1f;
                        Seconds++;

                        OnTick?.Invoke();
                    }
                }
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
    }
}