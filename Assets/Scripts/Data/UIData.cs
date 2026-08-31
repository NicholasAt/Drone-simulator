using Assets.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(menuName = "Data/UI data")]
    public class UIData : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject MainMenuWindowReference{ get; private set; }
    }
}