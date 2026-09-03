using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Windows.Popup
{
    public class PopupTwoButtons : BasePopup
    {
        [SerializeField] private UIStars _uIStars;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _leftButtonText, _rightButtonText;
        [SerializeField] private Button _leftButton, _rightButton;

        public Action OnLeftButtonClick { get; set; }
        public Action OnRightButtonClick { get; set; }

        private void Awake()
        {
            _leftButton.onClick.AddListener(() => OnLeftButtonClick?.Invoke());
            _rightButton.onClick.AddListener(() => OnRightButtonClick?.Invoke());
        }

        public void Refresh(string title, string leftText, string rightText)
        {
            _titleText.text = title;
            _leftButtonText.text = leftText;
            _rightButtonText.text = rightText;
        }
        public void RefreshStars(int amount)
        {
            _uIStars.RefreshStars(amount);
        }
        public void ShowHideStars(bool isShow)
        {
            _uIStars.ShowHideStars(isShow);
        }
        protected override void OnClose()
        {
            _leftButton.onClick.RemoveAllListeners();
            _rightButton.onClick.RemoveAllListeners();
            Destroy(gameObject);
        }
    }
}