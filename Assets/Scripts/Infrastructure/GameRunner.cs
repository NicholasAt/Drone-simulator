using Assets.Scripts.Services.GameStates;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Infrastructure
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] private GameObject[] _dontDestroyOnLoad;
        private GameStateMachine _stateMachine;

        [Inject]
        private void Constuct(GameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public async UniTask Run()
        {
            foreach (GameObject obj in _dontDestroyOnLoad)
            {
                obj.transform.parent = null;
                DontDestroyOnLoad(obj);
            }
            await _stateMachine.LoadMainMenu();
        }
    }
}