using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Data.DestroyVehiclesEffect
{
    [Serializable]
    public class VehiclesDestroyEffectConfig
    {
        [field: SerializeField] public DestroyEffectId EffectId { get; private set; }
        [field: SerializeField] public float Force { get; private set; } = 25;
        [field: SerializeField] public float Torque { get; private set; } = 5;
        [field: SerializeField] public float CircleRadius { get; private set; } = 10;
        [field: SerializeField] public float ConeHeight { get; private set; } = 5;
        [field: SerializeField] public AssetReferenceGameObject Reference { get; private set; }
    }
}