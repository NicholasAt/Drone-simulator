using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(menuName = "Data/UI data")]
    public class UIData : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject MainMenuWindowReference { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject HUDReference { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject PopUpTwoButtonsReference { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject PopUpMessageReference { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject ScreenTargetWindowReference { get; private set; }
    }
}