using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Data.DronesData
{
    [Serializable]
    public class DroneConfig
    {
        [field: SerializeField] public DroneID DroneID { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject DroneReference { get; private set; }
    }
}