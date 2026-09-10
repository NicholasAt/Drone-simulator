using Assets.Scripts.Data;
using Assets.Scripts.Services.GameProgress;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts
{
    public class AudioMixerService
    {
        private readonly ProgressService _progressService;
        private readonly GameData _gameData;
        private AudioMixer _mainMixer;
        private MusicProgress _musicProgress;

        public AudioMixerService(ProgressService progressService, GameData gameData)
        {
            _progressService = progressService;
            _gameData = gameData;
        }
        public void Init()
        {
            _musicProgress = _progressService.MusicProgress;
            _mainMixer = _gameData.MainAudioMixer;

            _musicProgress.OnChange += Refresh;
            Refresh();
        }

        private void Refresh()
        {
            float music = _musicProgress.MusicEnable ? 1 : 0;
            float sound = _musicProgress.SoundEnable ? 1 : 0;

            float musicVolume = Mathf.Log10(Mathf.Max(music, 0.0001f)) * 20f;
            float soundVolume = Mathf.Log10(Mathf.Max(sound, 0.0001f)) * 20f;

            _mainMixer.SetFloat(Constants.MusicSettings.MusicVolume, musicVolume);
            _mainMixer.SetFloat(Constants.MusicSettings.SFXVolume, soundVolume);
        }
    }
}