using Assets.Scripts.References;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Logic
{
    public class MusicSourceKeeper : MonoBehaviour
    {
        [field: SerializeField] public AudioSource BGSource { get; private set; }
        [field: SerializeField] public AudioSource SFXSource { get; private set; }
        [field: SerializeField] public AudioMixer MainAudioMixer { get; private set; }
    }
}