using Assets.Scripts.Data;
using Assets.Scripts.Data.Quests;
using Assets.Scripts.Extensions;
using Assets.Scripts.Services;
using Assets.Scripts.Services.GameProgress;
using Assets.Scripts.Services.GameStates;
using Assets.Scripts.Services.ServiceAnalytics;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI.Windows.UIHUD
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private Button _menuButton;
        [SerializeField] private TMP_Text _guideText;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _speedText;

        private UIFactory _uIFactory;
        private GameStateMachine _gameStateMachine;
        private TimerService _timerService;
        private GameObserver _gameObserver;
        private TempLevelProgress _levelProgress;
        private GameData _gameData;
        private IAnalytics _analytics;
        private bool _inProcess;

        [Inject]
        private void Construct(UIFactory uIFactory, GameStateMachine gameStateMachine, TimerService timerService, GameObserver gameObserver, ProgressService progressService, GameData gameData, Services.ServiceAnalytics.IAnalytics analytics)
        {
            _uIFactory = uIFactory;
            _gameStateMachine = gameStateMachine;
            _timerService = timerService;
            _gameObserver = gameObserver;
            _levelProgress = progressService.TempLevelProgress;
            _gameData = gameData;
            _analytics = analytics;
        }

        private void Start()
        {
            _menuButton.onClick.AddListener(() => HomePopup().Forget(Debug.LogError));
            _timerService.OnTick += RefreshTimer;
            RefreshTrasnportGuide();
        }
        private void Update()
        {
            _speedText.text = Mathf.RoundToInt(_gameObserver.CharacterSpeed).ToString();
        }
        private void OnDestroy()
        {
            _menuButton.onClick.RemoveAllListeners();
            _timerService.OnTick -= RefreshTimer;
        }

        private void RefreshTimer()
        {
            _timerText.text = _timerService.Seconds.ToTime();
        }

        private async UniTask HomePopup()
        {
            if (_inProcess)
                return;
            _inProcess = true;

            try
            {
                _gameStateMachine.SetPause(true);
                Popup.PopupTwoButtons popup = await _uIFactory.CreatePopupTwoButtons(this.GetCancellationTokenOnDestroy());

                popup.OnLeftButtonClick += () => LoadMenu().Forget();
                popup.OnRightButtonClick += () => ContinueGame(popup);
                popup.ShowHideStars(false);
                popup.Refresh("Return to Menu?", "Yes", "No");
            }

            finally
            {
                _inProcess = false;
            }
        }

        private void ContinueGame(Popup.PopupTwoButtons popup)
        {
            popup.Close();
            _gameStateMachine.SetPause(false);
        }

        private async UniTask LoadMenu()
        {
            if (_inProcess)
                return;
            _inProcess = true;

            try
            {
                _analytics.LeaveMission(_levelProgress.QuestID, _timerService.Seconds.ToTime());
                await _gameStateMachine.LoadMainMenu();
            }

            finally
            {
                _inProcess = false;
            }
        }
        private void RefreshTrasnportGuide()
        {
            if (_gameData.IsMobile())
            {
                _guideText.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                _guideText.transform.parent.gameObject.SetActive(true);
                switch (_levelProgress.QuestID)
                {
                    case QuestID.Helicopter_Move:
                    case QuestID.Helicopter_Delivery:
                        _guideText.text = "WASD  Q/E  Shift/Space";
                        break;

                    case QuestID.Drone_Move:
                    case QuestID.Drone_DestroyMovingCar:
                    case QuestID.Drone_DestroyFlyingObjects:
                    case QuestID.Drone_DestroyStatic:
                        _guideText.text = "WASD  Q/E  Space";
                        break;

                    case QuestID.None:
                    default:
                        Debug.LogError("no id");
                        break;
                }
            }
        }
    }
}