using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(menuName = "Data/Game data")]
    public class GameData : ScriptableObject
    {
        [SerializeField] private bool _isMobilePlatform;
        [field: SerializeField] public int FrameRate { get; private set; } = 60;
        public bool IsMobile()
        {
            return _isMobilePlatform || Application.isMobilePlatform;
        }
    }
}