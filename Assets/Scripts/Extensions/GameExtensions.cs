using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class GameExtensions
    {
        public static string ToKm(this float meters)
        {
            meters = Mathf.Ceil(meters);
            if (meters >= 1000)
            {
                float value = Mathf.Floor(meters / 100f) / 10f;
                return $"{value} Km";
            }
            return $"{meters} M";
        }
    }
}