using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class WindowAnimation : MonoBehaviour
    {
        [SerializeField] private float _animationSpeed = 6;
        [SerializeField] private bool _deactivateOnHide;
        [SerializeField] private CanvasGroup _canvasGroup;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            _canvasGroup.alpha = 0;
        }
        private void OnDestroy()
        {
            Close();
        }
        public async UniTask Show()
        {
            await Play(true);
        }

        public async UniTask Hide()
        {
            await Play(false);
        }

        private async UniTask Play(bool isShow)
        {
            Close();
            _cts = new CancellationTokenSource();
            CancellationToken ct = _cts.Token;

            try
            {
                if (isShow && _deactivateOnHide)
                    gameObject.SetActive(true);

                float endValue = isShow ? 1 : 0;
                while (ct.IsCancellationRequested == false)
                {
                    float alpha = _canvasGroup.alpha;
                    alpha = Mathf.MoveTowards(alpha, endValue, _animationSpeed * Time.deltaTime);

                    if (Mathf.Approximately(endValue, alpha))
                    {
                        _canvasGroup.alpha = endValue;
                        if (isShow == false)
                        {
                            if (_deactivateOnHide)
                                gameObject.SetActive(false);
                        }

                        break;
                    }
                    else
                        _canvasGroup.alpha = alpha;
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }
            }
            catch (OperationCanceledException)
            {
                //ignore
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
        private void Close()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
    }
}