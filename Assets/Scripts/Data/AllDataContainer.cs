using Assets.Scripts.Data.Quests;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(menuName = "Data/All data")]
    public class AllDataContainer : ScriptableObject
    {
        [field: SerializeField] public DroneData DroneData { get; private set; }
        [field: SerializeField] public UIData UIData { get; private set; }
        [field: SerializeField] public LocationData LocationData { get; private set; }
        [field: SerializeField] public QuestsData QuestsData { get; private set; }
        [field: SerializeField] public QuestObjectsData QuestObjectsData { get; private set; }
    }
}