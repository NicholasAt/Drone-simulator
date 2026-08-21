using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data.HelicoptersData
{
    [CreateAssetMenu(menuName = "Data/Helicopter")]
    public class HelicopterData : ScriptableObject
    {
        [SerializeField] private List<HelicopterConfig> _configs;
    }
}