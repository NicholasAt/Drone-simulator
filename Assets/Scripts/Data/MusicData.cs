using Assets.Scripts.References;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(menuName = "Data/Music data")]
    public class MusicData : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject AudioSourceReference { get; private set; }
        [field: SerializeField] public AssetReferenceAudioClip BackgroundClipReference { get; private set; }
        [field: SerializeField] public AssetReferenceAudioClip ButtonClipReference { get; private set; }

    }
}