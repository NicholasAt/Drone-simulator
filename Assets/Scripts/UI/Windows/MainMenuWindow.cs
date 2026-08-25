using Assets.Scripts.Data.Quests;
using Assets.Scripts.Services;
using Assets.Scripts.Services.GameProgress;
using Assets.Scripts.Services.GameStates;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI.Windows
{
    public class MainMenuWindow : MonoBehaviour
    {
        [SerializeField] private Button _quest1Button, _quest2Button,_quest3Buttone;
        private GameStateMachine _stateMachine;
        private UIFactory _uIFactory;
        private TempLevelProgress _levelProgress;

        [Inject]
        private void Construct(GameStateMachine gameStateMachine, UIFactory uIFactory, ProgressService progressService)
        {
            _stateMachine = gameStateMachine;
            _uIFactory = uIFactory;
            _levelProgress = progressService.TempLevelProgress;
        }

        private void Start()
        {
            _quest1Button.onClick.AddListener(() => LoadGame(QuestID.DroneMove).Forget());
            _quest2Button.onClick.AddListener(() => LoadGame(QuestID.HelicopterMove).Forget());
            _quest3Buttone.onClick.AddListener(() => LoadGame(QuestID.HelicopterDelivery).Forget());
        }

        private async UniTask LoadGame(QuestID id)
        {
            _quest1Button.onClick.RemoveAllListeners();
            _quest2Button.onClick.RemoveAllListeners();
            _quest3Buttone.onClick.RemoveAllListeners();

            _levelProgress.SetQuestId(id);
            await _stateMachine.LoadLocation1();
        }
    }
}