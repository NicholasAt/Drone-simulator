using Assets.Scripts.Services.GameStates;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI.Windows.UIHUD
{
    public class PopupHome : MonoBehaviour
    {
        [SerializeField] private Button _yesButton, _noButton;
        private GameStateMachine _stateMachine;
        private bool _loading;

        [Inject]
        private void Construct(GameStateMachine gameStateMachine)
        {
            _stateMachine = gameStateMachine;
        }

        private void Start()
        {
            _yesButton.onClick.AddListener(() => LoadMenu().Forget(Debug.LogError));
            _noButton.onClick.AddListener(Close);
        }

        private void OnDestroy()
        {
            _yesButton.onClick.RemoveAllListeners();
            _noButton.onClick.RemoveAllListeners();
        }

        private async UniTask LoadMenu()
        {
            if (_loading)
                return;
            _loading = true;

            try
            {
                await _stateMachine.LoadMainMenu();
            }
            finally
            {
                _loading = false;
            }
        }
        private void Close()
        {
            if (_loading)
                return;
            _loading = true;
            Destroy(gameObject);
        }
    }
}