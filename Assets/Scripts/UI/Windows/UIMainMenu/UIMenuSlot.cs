using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Windows.UIMainMenu
{
    public class UIMenuSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private UIStars _uIStars;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _selectImage;
        [SerializeField] private Color _selectColor, _defaultColor;
        public object Id { get; private set; }
        public Action OnClick { get; set; }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }
        public void SetId(object id)
        {
            Id = id;
        }
        public void Refresh(string title, Sprite icon)
        {
            _titleText.text = title;
            _iconImage.sprite = icon;
        }
        public void SetSelect(bool isSelect)
        {
            _selectImage.color = isSelect ? _selectColor : _defaultColor;
        }
        public void RefreshStars(int enableCount)
        {
            _uIStars.RefreshStars(enableCount);
        }
        public void ShowHideStars(bool isShow)
        {
            _uIStars.ShowHideStars(isShow);
        }
        public void Close()
        {
            Destroy(gameObject);
        }
    }
}