using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Windows
{
    public class UIStars : MonoBehaviour
    {
        [SerializeField] private Color _hideStarsColor, _showStarsColor;
        [SerializeField] private Image[] _stars;

        public void RefreshStars(int enableCount)
        {
            for (int i = 0; i < _stars.Length; i++)
            {
                bool isEnable = enableCount - 1 >= i;
                _stars[i].color = isEnable ? _showStarsColor : _hideStarsColor;
            }
        }

        public void ShowHideStars(bool isShow)
        {
            foreach (Image star in _stars)
            {
                star.gameObject.SetActive(isShow);
            }
        }
    }
}