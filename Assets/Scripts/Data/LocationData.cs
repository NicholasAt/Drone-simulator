using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(menuName = "Data/Location data")]
    public class LocationData : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject BrainReference { get; private set; }
    }
}