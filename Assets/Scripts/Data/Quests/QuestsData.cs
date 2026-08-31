using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data.Quests
{
    [CreateAssetMenu(menuName = "Data/Quests/Data")]
    public class QuestsData : ScriptableObject
    {
        [SerializeField] private List<QuestCategory> _categoryConfigs;
        public IList<QuestCategory> CategoryConfigs => _categoryConfigs;

        public QuestCategory GetCategoryByQuestId(QuestID id)
        {
            foreach (QuestCategory categoryConfig in _categoryConfigs)
            {
                foreach (QuestConfig questConfig in categoryConfig.QuestConfigs)
                {
                    if (questConfig.QuestID == id)
                        return categoryConfig;
                }
            }
            Debug.LogError($"no cfg [{id}]");
            return null;
        }
        public QuestConfig GetQuest(QuestID iD)
        {
            foreach (QuestCategory categoryConfig in _categoryConfigs)
            {
                foreach (QuestConfig cfg in categoryConfig.QuestConfigs)
                {
                    if (cfg.QuestID == iD)
                        return cfg;
                }
            }
            Debug.LogError($"no id [{iD}]");
            return null;
        }
    }
}