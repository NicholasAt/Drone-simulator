using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Windows.Popup
{
    public class PopupWarningOneButton : BasePopup
    {
        [SerializeField] private WindowAnimation _windowAnimation;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _buttonText;
        [SerializeField] private Button _button;

        public Action OnButtonClick { get; set; }

        private void Start()
        {
            _windowAnimation.Show().Forget();
            _button.onClick.AddListener(() => OnButtonClick?.Invoke());
        }

        public void Refresh(string title, string buttonText)
        {
            _titleText.text = title;
            _buttonText.text = buttonText;
        }

        protected override void OnClose()
        {
            _button.onClick.RemoveAllListeners();
            _windowAnimation.Hide().ContinueWith(() => Destroy(gameObject));
        }
    }
}