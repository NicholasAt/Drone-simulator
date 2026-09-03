using Assets.Scripts.Services;
using Assets.Scripts.Services.GameStates;
using Assets.Scripts.UI.Windows.Popup;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public abstract class BaseQuest : MonoBehaviour
    {
        private GameStateMachine _gameStateMachine;
        private UIFactory _uIFactory;

        private bool _baseIsEnd;
        private bool _baseInProcess;

        [Inject]
        private void Construct(GameStateMachine gameStateMachine, UIFactory uIFactory)
        {
            _gameStateMachine = gameStateMachine;
            _uIFactory = uIFactory;
        }
        public async UniTask Run()
        {
            await OnRun();
        }

        protected virtual async UniTask OnRun()
        {
            await UniTask.CompletedTask;
        }

        protected async UniTask ProtectedLose(int stars, string message = "")
        {
            await WinLose(false, stars, message);
        }
        protected async UniTask ProtectedWin(int stars, string message = "")
        {
            await WinLose(true, stars, message);
        }
        private async UniTask WinLose(bool isWin, int stars, string title = "")
        {
            if (_baseIsEnd)
                return;

            _baseIsEnd = true;
            _gameStateMachine.SetPause(true);

            PopupTwoButtons popup = await _uIFactory.CreatePopupTwoButtons(this.GetCancellationTokenOnDestroy());

            popup.OnLeftButtonClick += ToMainMenu;
            popup.OnRightButtonClick += Restart;

            if (string.IsNullOrEmpty(title))
                title = isWin ? "Congratulations" : "Lose";
            popup.Refresh(title, "To Menu", "Restart");

            if (isWin)
                popup.RefreshStars(stars);
            else
                popup.RefreshStars(0);
        }

        private void Restart()
        {
            if (_baseInProcess)
                return;

            _baseInProcess = true;
            _gameStateMachine.LoadLocation1().Forget(Debug.LogError);
        }

        private void ToMainMenu()
        {
            if (_baseInProcess)
                return;

            _baseInProcess = true;
            _gameStateMachine.LoadMainMenu().Forget(Debug.LogError);
        }
    }
}