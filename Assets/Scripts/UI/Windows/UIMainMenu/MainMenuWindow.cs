using Assets.Scripts.Data;
using Assets.Scripts.Data.Quests;
using Assets.Scripts.Services;
using Assets.Scripts.Services.GameProgress;
using Assets.Scripts.Services.GameStates;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI.Windows.UIMainMenu
{
    public class MainMenuWindow : MonoBehaviour
    {
        [SerializeField] private UIMenuSlot _transportFieldTemplate;
        [SerializeField] private UIMenuSlot _missionFieldTemplate;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Button _playButton;

        private GameStateMachine _stateMachine;
        private UIFactory _uIFactory;
        private TempLevelProgress _levelProgress;
        private StartsProgress _starsProgress;
        private MusicProgress _musicProgress;
        private QuestsData _questsData;
        private UIData _uIData;
        private bool _triggered;
        private readonly List<UIMenuSlot> _transportSlots = new();
        private readonly List<UIMenuSlot> _missionSlots = new();

        [Inject]
        private void Construct(GameStateMachine gameStateMachine, UIFactory uIFactory, QuestsData questsData, ProgressService progressService,UIData uIData)
        {
            _stateMachine = gameStateMachine;
            _uIFactory = uIFactory;
            _levelProgress = progressService.TempLevelProgress;
            _starsProgress = progressService.StartsProgress;
            _musicProgress = progressService.MusicProgress;
            _questsData = questsData;
            _uIData = uIData;
        }

        private void Start()
        {
            InitSlots();
            _playButton.onClick.AddListener(() => LoadGame().Forget(Debug.LogError));
        }
       
        private void InitSlots()
        {
            foreach (QuestCategory categoryConfig in _questsData.CategoryConfigs)
            {
                UIMenuSlot slot = Instantiate(_transportFieldTemplate, _transportFieldTemplate.transform.parent);
                _transportSlots.Add(slot);
                slot.gameObject.SetActive(true);
                
                slot.SetColors(_uIData.SelectColor, _uIData.DefaultColor);
                slot.SetId(categoryConfig.TransportName);
                slot.Refresh(categoryConfig.TransportName, categoryConfig.Icon);
                slot.ShowHideStars(false);
                slot.OnClick += () => SetQuestCategory(categoryConfig);
            }

            RefreshTransports();
            RefreshMission();
        }

        private void RefreshTransports()
        {
            foreach (UIMenuSlot slot in _transportSlots)
            {
                QuestCategory category = _questsData.GetCategoryByQuestId(_levelProgress.QuestID);
                bool isSelect = slot.Id is string transportName && transportName == category.TransportName;
                slot.SetSelect(isSelect);
            }
        }
        private void RefreshMission()
        {
            _missionSlots.ForEach(slot => slot.Close());
            _missionSlots.Clear();

            QuestCategory category = _questsData.GetCategoryByQuestId(_levelProgress.QuestID);
            foreach (QuestConfig cfg in category.QuestConfigs)
            {
                UIMenuSlot slot = Instantiate(_missionFieldTemplate, _missionFieldTemplate.transform.parent);
                slot.gameObject.SetActive(true);
                _missionSlots.Add(slot);

                slot.SetColors(_uIData.SelectColor, _uIData.DefaultColor);
                slot.SetId(cfg.QuestID);
                slot.Refresh(cfg.MissionName, cfg.Icon);
                slot.RefreshStars(_starsProgress.GetStars(cfg.QuestID));
                slot.OnClick += () => SetQuestId(cfg.QuestID);
            }
            RefreshMissionSelect();
        }

        private void RefreshMissionSelect()
        {
            QuestID currentId = _levelProgress.QuestID;
            foreach (UIMenuSlot slot in _missionSlots)
            {
                bool isSelect = slot.Id is QuestID id && id == currentId;
                slot.SetSelect(isSelect);
            }
            RefreshMissionDescription();
        }
        private void RefreshMissionDescription()
        {
            QuestID currentId = _levelProgress.QuestID;
            QuestConfig cfg = _questsData.GetQuest(currentId);
            _descriptionText.text = cfg.MissionDescription;
        }

        private void SetQuestCategory(QuestCategory category)
        {
            _levelProgress.SetQuestId(category.QuestConfigs[0].QuestID);
            RefreshTransports();
            RefreshMission();
        }

        private void SetQuestId(QuestID id)
        {
            _levelProgress.SetQuestId(id);
            RefreshMissionSelect();
            RefreshMissionDescription();
        }

        private async UniTask LoadGame()
        {
            if (_triggered)
                return;
            _triggered = true;

            await _stateMachine.LoadLocation1();
        }
    }
}