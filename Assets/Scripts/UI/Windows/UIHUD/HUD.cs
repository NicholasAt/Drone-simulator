using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI.Windows.UIHUD
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private Button _menuButton;
        private UIFactory _uIFactory;
        private bool _inProcess;

        [Inject]
        private void Construct(UIFactory uIFactory)
        {
            _uIFactory = uIFactory;
        }

        private void Start()
        {
            _menuButton.onClick.AddListener(() => HomePopup().Forget(Debug.LogError));
        }
        private void OnDestroy()
        {
            _menuButton.onClick.RemoveAllListeners();
        }

        private async UniTask HomePopup()
        {
            if (_inProcess)
                return;
            _inProcess = true;

            try
            {
                await _uIFactory.CreatePopupHome();
            }
            finally
            {
                _inProcess = false;
            }
        }
    }
}