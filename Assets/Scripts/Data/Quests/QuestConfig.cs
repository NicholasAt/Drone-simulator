using Assets.Scripts.Quests.Scenarios;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Data.Quests
{
    [Serializable]
    public class QuestConfig
    {
        [field: SerializeField, TextArea] public string MissionName { get; private set; }
        [field: SerializeField, TextArea] public string MissionDescription { get; private set; }

        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public QuestID QuestID { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject QuestReference { get; private set; }
    }

    [Serializable]
    public class QuestCategory
    {
        [field: SerializeField] public string TransportName { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [SerializeField] private List<QuestConfig> _questConfig;
        public IList<QuestConfig> QuestConfigs => _questConfig;
    }
}