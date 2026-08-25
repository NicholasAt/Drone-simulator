using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Data.Quests
{
    [CreateAssetMenu(menuName = "Data/Quests/Objects data")]
    public class QuestObjectsData : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject QuestPointReference { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject DeliveryItemReference { get; private set; }
    }
}