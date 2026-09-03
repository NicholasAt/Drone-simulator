using UnityEngine;

namespace Assets.Scripts.Services
{
    public class CalculateStarsService
    {
        private const int MaxStars = 5;
        private const int MinStars = 1;

        public int Calculate(int badRecord, int bestRecord, int current)
        {
            int result;
            if (current <= bestRecord)
                result = MaxStars;
            else if (current >= badRecord)
                result = MinStars;
            else
            {
                float lerp = Mathf.InverseLerp(bestRecord, badRecord, current);
                result = Mathf.RoundToInt(Mathf.Lerp(MaxStars, MinStars, lerp));
            }
            return result;
        }
    }
}