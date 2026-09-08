using Assets.Scripts.Character;
using Assets.Scripts.Logic;
using Assets.Scripts.Services;
using Assets.Scripts.Services.CameraService;
using Assets.Scripts.Services.GameProgress;
using Assets.Scripts.Services.GameStates;
using Assets.Scripts.UI.Windows.Popup;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Quests.Scenarios
{
    public interface IScenario
    {
        UniTask Run();
    }
    public abstract class BaseQuest<TConfig> : MonoBehaviour, IScenario where TConfig : BaseQuest<TConfig>.BaseConfig
    {
        [Serializable]
        public class BaseConfig
        {
            [TextArea] public string TransportDestroyedMessage;
            public float DieImpulse = 20;
            public float PlayerDamageRadius = 5;
        }
        [SerializeField] protected TConfig Config;

        protected GameStateMachine GameStateMachine;

        protected CharacterComponentsKeeperService ComponentsKeeper;

        protected UIFactory UIFactory;
        protected CameraStateService CameraState;
        protected TransportFactory TransportFactory;
        protected HitHandler HitHandler;
        protected TempLevelProgress LevelProgress;

        protected ShowKills ShowKills;
        protected PopupMessage PopupMessage;

        private bool _baseIsEnd;
        private bool _baseInProcess;


        [Inject]
        private void Construct(GameStateMachine gameStateMachine, CharacterComponentsKeeperService componentsKeeper, ShowKills showKills, ProgressService progressService, TransportFactory transportFactory, UIFactory uIFactory, CameraStateService cameraStateService, HitHandler hitHandler)
        {
            GameStateMachine = gameStateMachine;
            ComponentsKeeper = componentsKeeper;
            UIFactory = uIFactory;
            CameraState = cameraStateService;
            TransportFactory = transportFactory;
            HitHandler = hitHandler;
            LevelProgress = progressService.TempLevelProgress;
            ShowKills = showKills;
        }
        private void OnDestroy()
        {
            if (ComponentsKeeper.CharacterHit != null)
            {
                ComponentsKeeper.CharacterHit.OnHit -= OnPlayerHit;
                ComponentsKeeper.CharacterHit.OnTurned -= OnPlayerTurned;
            }
        }
        public async UniTask Run()
        {
            PopupMessage = await UIFactory.CreatePopupMessage(this.GetCancellationTokenOnDestroy());
            ShowKills.Init(PopupMessage);
            HitHandler.Init(Config.PlayerDamageRadius);

            (Vector3 pos, Quaternion rotate) = PositionAndRotate();

            await TransportFactory.CreateTransport(LevelProgress.QuestID, pos, rotate);
            ComponentsKeeper.CharacterHit.OnHit += OnPlayerHit;
            ComponentsKeeper.CharacterHit.OnTurned += OnPlayerTurned;

            await OnRun();
        }

        protected virtual async UniTask OnRun()
        {
            await UniTask.CompletedTask;
        }

        protected async UniTask Lose(int stars, string message = "")
        {
            await WinLose(false, stars, message);
        }
        protected async UniTask Win(int stars, string message = "")
        {
            await WinLose(true, stars, message);
        }
        protected void ShowPopupMessage(string message)
        {
            PopupMessage.Show(message);
        }
        protected abstract (Vector3 pos, Quaternion rotate) PositionAndRotate();

        protected virtual void OnPlayerTurned()
        {
            if (HitHandler.TryDamage(CharacterPos()))
            {
                ShowKills.Run(HitHandler.Targets);
                PlayAnimation(true);
            }
            else
            {
                ShowPopupMessage(Config.TransportDestroyedMessage);
                PlayAnimation(false);
            }
        }

        protected virtual void OnPlayerHit(float impulse)
        {
            if (HitHandler.TryDamage(CharacterPos()))
            {
                ShowKills.Run(HitHandler.Targets);
                PlayAnimation(true);
            }
            else if (impulse > Config.DieImpulse)
            {
                ShowPopupMessage(Config.TransportDestroyedMessage);
                PlayAnimation(false);
            }
        }
        protected virtual void PlayAnimation(bool isHit)
        {
            if (isHit == false)
                ComponentsKeeper.DestroyEffectPlayer.Play().Forget();

            (Vector3 pos, Quaternion rotate) = PositionAndRotate();
            ComponentsKeeper.CharacterRefresher.Hide();
            CameraState.Enter<CameraAnimationState, Vector3, Action, CancellationToken>(CharacterPos(), RestartPlayer, this.GetCancellationTokenOnDestroy()).Forget();
        }

        protected virtual void RestartPlayer()
        {
            (Vector3 pos, Quaternion rotate) = PositionAndRotate();
            CameraState.Enter<CameraToCharacterState>().Forget();
            ComponentsKeeper.CharacterRefresher.Show(pos, rotate);
        }
        protected virtual Vector3 CharacterPos()
        {
            return ComponentsKeeper.Pos();
        }
        private async UniTask WinLose(bool isWin, int stars, string title = "")
        {
            if (_baseIsEnd)
                return;

            _baseIsEnd = true;
            GameStateMachine.SetPause(true);

            PopupTwoButtons popup = await UIFactory.CreatePopupTwoButtons(this.GetCancellationTokenOnDestroy());

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
            GameStateMachine.LoadLocation1().Forget(Debug.LogError);
        }

        private void ToMainMenu()
        {
            if (_baseInProcess)
                return;

            _baseInProcess = true;
            GameStateMachine.LoadMainMenu().Forget(Debug.LogError);
        }
    }
}