using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(menuName = "Data/Out Screen Data")]
    public class OutScreenData : ScriptableObject
    {
        [field: SerializeField] public float RendererDistance { get; private set; } = 100;
        [field: SerializeField] public float EdgeOffset { get; private set; } = 50;
    }
}