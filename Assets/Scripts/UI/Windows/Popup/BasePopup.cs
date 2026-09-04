using UnityEngine;

namespace Assets.Scripts.UI.Windows.Popup
{
    public abstract class BasePopup : MonoBehaviour
    {
        public void Close()
        {
            OnClose();
        }
       
        protected virtual void OnClose() { }
    }
}