using UnityEngine;

namespace Assets.Scripts.Bots
{
    public class BotHide : MonoBehaviour, IRefreshPositions
    {
        public void Show(Vector3 pos, Quaternion rotate)
        {
            gameObject.SetActive(true);
            transform.SetParent(transform.parent);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}