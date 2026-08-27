using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data.BotsData.FlyData
{
    [CreateAssetMenu(menuName = "Data/Flying transport data")]
    public class FlyingTransportData : ScriptableObject
    {
        [SerializeField] private List<FlyingTransportConfig> _configs;
        public FlyingTransportConfig GetConfig(FlyingTransportID id)
        {
            foreach (FlyingTransportConfig cfg in _configs)
            {
                if (cfg.ID == id)
                    return cfg;
            }
            Debug.LogError("no cfg");
            return null;
        }
    }
}