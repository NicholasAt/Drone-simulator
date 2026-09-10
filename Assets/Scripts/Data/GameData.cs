using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(menuName = "Data/Game data")]
    public class GameData : ScriptableObject
    {
        [field: SerializeField] public AudioMixer MainAudioMixer { get; private set; }
    }
}