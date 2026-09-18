using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(menuName = "Data/UI data")]
    public class UIData : ScriptableObject
    {
        [field: SerializeField] public Color SelectBgColor { get; private set; }
        [field: SerializeField] public Color DefaultBgColor { get; private set; }
        [field: SerializeField] public Color SelectPointColor { get; private set; }
        [field: SerializeField] public Color DefaultPointColor { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject MainMenuWindowReference { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject HUDReference { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject PopUpTwoButtonsReference { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject PopUpMessageReference { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject ScreenTargetWindowReference { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject LoadingCurtainReference { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject MobileInputReference { get; private set; }
        public float AnimationSpeed {  get; private set; }  
        public void SetAnimationSpeed(float speed)
        {
            AnimationSpeed = speed;
        }
    }
}