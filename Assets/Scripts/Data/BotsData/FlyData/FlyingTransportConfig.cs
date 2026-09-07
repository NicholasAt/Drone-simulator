using Assets.Scripts.Data.DestroyVehiclesEffect;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Data.BotsData.FlyData
{
    [Serializable]
    public class FlyingTransportConfig
    {
        [field: SerializeField] public string TransportName { get; private set; }
        [field: SerializeField] public FlyingTransportID ID { get; private set; }
        [field: SerializeField] public DestroyEffectId EffectId { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject PrefabReference { get; private set; }
        [field: SerializeField] public float Speed { get; private set; } = 5;
    }
}