using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(menuName = "Data/Drones")]
    public class DroneData : ScriptableObject
    {
        [Serializable]
        public class DroneConfig
        {
            [field: SerializeField] public DroneID DroneID { get; private set; }
            [field: SerializeField] public AssetReferenceGameObject DroneReference { get; private set; }
        }
        [SerializeField] private List<DroneConfig> _doneConfigs;
        public IList<DroneConfig> DroneConfigs => _doneConfigs;

    }
    public enum DroneID
    {
        None,
        Drone1
    }
}