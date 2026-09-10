using Assets.Scripts.Data;
using Assets.Scripts.Logic;
using Assets.Scripts.Services.AssetProvider;
using Assets.Scripts.Services.GameProgress;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace Assets.Scripts
{
    public class MusicService
    {
        private readonly ProgressService _progressService;
        private readonly MusicData _musicData;
        private readonly IAssetProviderService _assetProviderService;
        private AudioClip _buttonClip;
        private MusicProgress _musicProgress;
        private AudioSource _sfxSource;
        private AudioSource _bgSource;
        private AudioMixer _mainMixer;

        public MusicService(ProgressService progressService, MusicData musicData, IAssetProviderService assetProviderService)
        {
            _progressService = progressService;
            _musicData = musicData;
            _assetProviderService = assetProviderService;
        }
        public void Stop()
        {
            _bgSource.Stop();
            _bgSource.clip = null;
        }

        public async UniTask Init()
        {
            await InitPlayer();
            _buttonClip = await _assetProviderService.LoadAsync<AudioClip>(_musicData.ButtonClipReference);

            _musicProgress = _progressService.MusicProgress;
            _musicProgress.OnChange += Refresh;
            Refresh();
        }

        public async UniTask PlayBackground()
        {
            AudioClip clip = await _assetProviderService.LoadAsync<AudioClip>(_musicData.BackgroundClipReference);
            _bgSource.clip = clip;
            _bgSource.Play();
        }

        public void Beep()
        {
            _sfxSource.PlayOneShot(_buttonClip);
        }
        private async UniTask InitPlayer()
        {
            GameObject prefab = await Addressables.LoadAssetAsync<GameObject>(_musicData.AudioSourceReference);//ignore release
            GameObject instance = Object.Instantiate(prefab);
            Object.DontDestroyOnLoad(instance);

            MusicSourceKeeper keeper = instance.GetComponent<MusicSourceKeeper>();
            _sfxSource = keeper.SFXSource;
            _bgSource = keeper.BGSource;
            _mainMixer = keeper.MainAudioMixer;
        }

        private void Refresh()
        {
            float music = _musicProgress.MusicEnable ? 1 : 0;
            float sound = _musicProgress.SFXEnable ? 1 : 0;

            float musicVolume = Mathf.Log10(Mathf.Max(music, 0.0001f)) * 20f;
            float soundVolume = Mathf.Log10(Mathf.Max(sound, 0.0001f)) * 20f;

            _mainMixer.SetFloat(Constants.MusicSettings.MusicVolume, musicVolume);
            _mainMixer.SetFloat(Constants.MusicSettings.SFXVolume, soundVolume);
        }
    }
}