using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Data.CameraAnimationData
{
    [CreateAssetMenu(menuName = "Data/Camera animation")]
    public class CameraData : ScriptableObject
    {
        [field: SerializeField] public LayerMask IgnoreMask { get; private set; }
        [field: SerializeField] public float Duration { get; private set; } = 2;
        [field: SerializeField] public float Width { get; private set; } = 55;
        [field: SerializeField] public float Height { get; private set; } = 35;
        [field: SerializeField] public AssetReferenceGameObject CinemaCameraReference { get; private set; }
    }
}