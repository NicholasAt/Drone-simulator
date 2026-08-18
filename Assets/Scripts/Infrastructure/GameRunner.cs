using Assets.Scripts.Services.GameStates;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Infrastructure
{
    public class GameRunner : MonoBehaviour
    {
        private GameStateMachine _stateMachine;

        [Inject]
        private void Constuct(GameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public async UniTask Run()
        {
           await _stateMachine.LoadMainMenu();
        }
    }
}