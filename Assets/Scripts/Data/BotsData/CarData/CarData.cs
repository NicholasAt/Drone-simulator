using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data.BotsData.CarData
{
    [CreateAssetMenu(menuName = "Data/Car data")]
    public class CarData : ScriptableObject
    {
        [SerializeField] private List<CarConfig> _carConfigs;

        public CarConfig GetConfig(CarID id)
        {
            foreach (CarConfig cfg in _carConfigs)
            {
                if (cfg.ID == id)
                    return cfg;
            }
            Debug.LogError($"no config [{id}]");
            return null;
        }
    }
}