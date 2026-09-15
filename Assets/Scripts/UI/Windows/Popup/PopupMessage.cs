using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Windows.Popup
{
    public class PopupMessage : BasePopup
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private WindowAnimation _windowAnimation;
        private CancellationTokenSource _cts;

        private void OnDestroy()
        {
            Stop();
        }
        public void Show(string message, float lifeTime = 3)
        {
            Stop();
            _cts = new CancellationTokenSource();
            _windowAnimation.Show().Forget();
            Run(lifeTime, _cts.Token).Forget();

            _title.text = message;
        }

        private async UniTaskVoid Run(float lifeTime, CancellationToken ct)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(lifeTime), cancellationToken: ct);
                _windowAnimation.Hide().Forget();
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

        private void Stop()
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