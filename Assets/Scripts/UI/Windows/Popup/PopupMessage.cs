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
        private float _currentTime;

        public void Show(string message, float lifeTime = 3)
        {
            if (gameObject.activeInHierarchy == false)
            {
                gameObject.SetActive(true);
                Run(this.GetCancellationTokenOnDestroy()).Forget();
            }
            _currentTime = lifeTime;
            _title.text = message;
        }

        private async UniTaskVoid Run(CancellationToken ct)
        {
            try
            {
                while (ct.IsCancellationRequested == false)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                    _currentTime -= Time.deltaTime;
                    if (_currentTime < 0)
                    {
                        gameObject.SetActive(false);
                        break;
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