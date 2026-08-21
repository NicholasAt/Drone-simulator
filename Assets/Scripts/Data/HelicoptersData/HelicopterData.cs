using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data.HelicoptersData
{
    [CreateAssetMenu(menuName = "Data/Helicopter")]
    public class HelicopterData : ScriptableObject
    {
        [SerializeField] private List<HelicopterConfig> _configs;
        public HelicopterConfig GetConfig(HelicopterID id)
        {
            foreach (HelicopterConfig cfg in _configs)
            {
                if (cfg.ID == id)
                    return cfg;
            }

            Debug.LogError($"no cfg [{id}]");
            return null;
        }
    }
}