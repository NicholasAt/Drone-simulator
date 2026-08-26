using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Data.BotsData.CarData
{
    [Serializable]
    public class CarConfig
    {
        [field: SerializeField] public CarID ID { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject PrefabReference { get; private set; }
        [field: SerializeField] public float Speed { get; private set; } = 5;
    }
}