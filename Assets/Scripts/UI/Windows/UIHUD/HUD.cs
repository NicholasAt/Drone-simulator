using Assets.Scripts.Services;
using Assets.Scripts.Services.GameStates;
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
        private GameStateMachine _gameStateMachine;
        private bool _inProcess;

        [Inject]
        private void Construct(UIFactory uIFactory, GameStateMachine gameStateMachine)
        {
            _uIFactory = uIFactory;
            _gameStateMachine = gameStateMachine;
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
                _gameStateMachine.Pause(true);
                await _uIFactory.CreatePopupHome(this.GetCancellationTokenOnDestroy());
            }
            
            finally
            {
                _inProcess = false;
            }
        }
    }
}