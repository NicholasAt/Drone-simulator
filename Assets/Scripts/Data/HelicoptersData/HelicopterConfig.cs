using Assets.Scripts.Data.DestroyVehiclesEffect;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Data.HelicoptersData
{
    [Serializable]
    public class HelicopterConfig
    {
        [field: SerializeField] public HelicopterID ID { get; private set; }
        [field: SerializeField] public DestroyEffectId DestroyEffect { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject HelicopterReference { get; private set; }
    }
}