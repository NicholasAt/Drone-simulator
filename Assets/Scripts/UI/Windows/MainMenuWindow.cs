using Assets.Scripts.Data.Quests;
using Assets.Scripts.Services;
using Assets.Scripts.Services.GameProgress;
using Assets.Scripts.Services.GameStates;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI.Windows
{
    public class MainMenuWindow : MonoBehaviour
    {
        [SerializeField] private Button _fieldButtonTemplate;
        private GameStateMachine _stateMachine;
        private UIFactory _uIFactory;
        private TempLevelProgress _levelProgress;
        private QuestsData _questsData;
        private bool _triggered;

        [Inject]
        private void Construct(GameStateMachine gameStateMachine, UIFactory uIFactory, QuestsData questsData, ProgressService progressService)
        {
            _stateMachine = gameStateMachine;
            _uIFactory = uIFactory;
            _levelProgress = progressService.TempLevelProgress;
            _questsData = questsData;
        }

        private void Start()
        {
            foreach (QuestConfig cfg in _questsData.QuestConfigs)
            {
                Button button = Instantiate(_fieldButtonTemplate, _fieldButtonTemplate.transform.parent);
                button.gameObject.SetActive(true);
                button.onClick.AddListener(() => LoadGame(cfg.QuestID).Forget());
                button.GetComponentInChildren<TMP_Text>().text = cfg.QuestID.ToString();
            }
        }

        private async UniTask LoadGame(QuestID id)
        {
            if (_triggered)
                return;
            _triggered = true;

            _levelProgress.SetQuestId(id);
            await _stateMachine.LoadLocation1();
        }
    }
}