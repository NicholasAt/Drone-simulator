using Assets.Scripts.Extensions;
using Assets.Scripts.Quests;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data.Quests
{
    [CreateAssetMenu(menuName = "Data/Quests/Data")]
    public class QuestsData : ScriptableObject
    {
        [SerializeField] private List<QuestConfig> _questConfigs;
        private void OnValidate()
        {
            _questConfigs.ForEach(cfg => cfg.OnValidate());
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
        public QuestConfig GetQuest(QuestID iD)
        {
            foreach (QuestConfig cfg in _questConfigs)
            {
                if (cfg.QuestID == iD)
                    return cfg;
            }
            Debug.LogError($"no id [{iD}]");
            return null;
        }
    }
    [Serializable]
    public class QuestConfig
    {
        [field: SerializeField] public QuestID QuestID { get; private set; }
        [field: SerializeField] public InterfaceReferenceGameObject<IQuest> QuestRunnerPrefab { get; private set; }

        public void OnValidate()
        {
            QuestRunnerPrefab.OnValidate();
        }
    }
    public enum QuestID
    {
        None,
        DroneMove = 1,
        HelicopterMove = 2,
    }
}