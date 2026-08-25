using Assets.Scripts.Extensions;
using Assets.Scripts.Quests;
using System;
using UnityEngine;

namespace Assets.Scripts.Data.Quests
{
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
}