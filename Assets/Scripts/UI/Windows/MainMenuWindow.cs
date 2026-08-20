using Assets.Scripts.Services;
using Assets.Scripts.Services.GameStates;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI.Windows
{
    public class MainMenuWindow : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        private GameStateMachine _stateMachine;
        private UIFactory _uIFactory;

        [Inject]
        private void Construct(GameStateMachine gameStateMachine, UIFactory uIFactory)
        {
            _stateMachine = gameStateMachine;
            _uIFactory = uIFactory;
        }

        private void Start()
        {
            _playButton.onClick.AddListener(() => LoadGame().Forget());
        }

        private async UniTask LoadGame()
        {
            _playButton.onClick.RemoveAllListeners();
            await _stateMachine.LoadLocation1();
        }
    }
}