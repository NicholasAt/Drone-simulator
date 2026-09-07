using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data.DestroyVehiclesEffect
{
    [CreateAssetMenu(menuName = "Data/Effects/Vehicles destroy")]
    public class VehiclesDestroyEffectData : ScriptableObject
    {
        [SerializeField] private List<VehiclesDestroyEffectConfig> _effectConfigs;
        [field: SerializeField] public float LifeSeconds { get; private set; } = 30;
        public VehiclesDestroyEffectConfig GetConfig(DestroyEffectId id)
        {
            foreach (VehiclesDestroyEffectConfig cfg in _effectConfigs)
            {
                if (cfg.EffectId == id)
                    return cfg;
            }
            Debug.LogError($"no cfg [{id}]");
            return null;
        }
    }
}