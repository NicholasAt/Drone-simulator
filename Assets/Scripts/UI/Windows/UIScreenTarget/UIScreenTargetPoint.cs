using Assets.Scripts.Extensions;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Windows.UIScreenTarget
{
    public class UIScreenTargetPoint : MonoBehaviour
    {
        [SerializeField] private TMP_Text _distanceText;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private GameObject _outScreenObject;
        [SerializeField] private GameObject _inScreenObject;

        public void Show(Vector3 pos, Vector3 direction)
        {
            _outScreenObject.SetActive(true);
            _inScreenObject.SetActive(false);
            _distanceText.gameObject.SetActive(true);

            _rectTransform.position = pos;
            _outScreenObject.transform.right = direction;
        }

        public void Show(Vector3 pos)
        {
            _outScreenObject.SetActive(false);
            _inScreenObject.SetActive(true);
            _distanceText.gameObject.SetActive(true);

            _rectTransform.position = pos;
        }
        public void UpdateDistance(float distance)
        {
            _distanceText.text = distance.ToKm();
        }
        public void Hide()
        {
            _outScreenObject.SetActive(false);
            _inScreenObject.SetActive(false);
            _distanceText.gameObject.SetActive(false);
        }
    }
}