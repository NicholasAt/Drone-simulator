using Assets.Scripts.Data;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.UI
{
    public class WindowAnimation : MonoBehaviour
    {
        [SerializeField] private bool _deactivateOnHide;
        [SerializeField] private CanvasGroup _canvasGroup;
        private CancellationTokenSource _cts;
        private UIData _uIData;

        [Inject]
        private void Construct(UIData uIData)
        {
            _uIData = uIData;
        }
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
                    float deltaTime = Mathf.Min(Time.deltaTime, 0.05f);
                    _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, endValue, _uIData.AnimationSpeed * deltaTime);

                    if (endValue == _canvasGroup.alpha)
                    {
                        if (isShow == false)
                        {
                            if (_deactivateOnHide)
                                gameObject.SetActive(false);
                        }
                        break;
                    }

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