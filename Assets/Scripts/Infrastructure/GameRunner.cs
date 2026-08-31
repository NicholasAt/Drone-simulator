using Assets.Scripts.Services.GameStates;
using Assets.Scripts.Services.InputService;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Infrastructure
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] private GameObject[] _dontDestroyOnLoad;
        private GameStateMachine _stateMachine;
        private IInputService _inputService;

        [Inject]
        private void Constuct(GameStateMachine stateMachine, IInputService inputService)
        {
            _stateMachine = stateMachine;
            _inputService = inputService;
        }

        public async UniTask Run()
        {
            _inputService.Init();
            foreach (GameObject obj in _dontDestroyOnLoad)
            {
                obj.transform.parent = null;
                DontDestroyOnLoad(obj);
            }
            await _stateMachine.LoadMainMenu();
        }
    }
}