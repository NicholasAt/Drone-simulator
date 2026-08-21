using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data.DronesData
{
    [CreateAssetMenu(menuName = "Data/Drones")]
    public class DroneData : ScriptableObject
    {
        [SerializeField] private List<DroneConfig> _droneConfigs;
        public DroneConfig GetConfig(DroneID id)
        {
            foreach (DroneConfig cfg in _droneConfigs)
            {
                if(cfg.DroneID == id)
                    return cfg;
            }
            Debug.LogError($"no cfg [{id}]");
            return null;
        }
    }
}