using UnityEngine;
using Zenject;

namespace Assets.Scripts.Bots
{
    public class BotHide : MonoBehaviour, IRefreshPositions
    {
        public void Show(Vector3 pos, Quaternion rotate)
        {
            Transform trans = transform;
            gameObject.SetActive(true);
            trans.SetParent(trans.parent);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}