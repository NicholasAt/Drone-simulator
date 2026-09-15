using Assets.Scripts.Data;
using Assets.Scripts.Services;
using Assets.Scripts.Services.GameProgress;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI.Windows.UIMainMenu
{
    public class MusicWindow : MonoBehaviour
    {
        [SerializeField] private Image _musicImage, _soundImage;
        [SerializeField] private Button _musicButton, _soundButton;

        private UIData _uIData;
        private MusicProgress _musicProgress;
        private MusicService _musicService;

        [Inject]
        private void Construct(UIData uIData, ProgressService progressService, MusicService musicService)
        {
            _uIData = uIData;
            _musicProgress = progressService.MusicProgress;
            _musicService = musicService;
        }
        private void Start()
        {
            Refresh();
            _musicButton.onClick.AddListener(ToggleMusic);
            _soundButton.onClick.AddListener(ToggleSound);
        }

        private void ToggleSound()
        {
            _musicProgress.ChangeSFX(!_musicProgress.SFXEnable);
            Refresh();
        }

        private void ToggleMusic()
        {
            _musicProgress.ChangeMusic(!_musicProgress.MusicEnable);
            Refresh();
        }

        private void Refresh()
        {
            _musicImage.color = _musicProgress.MusicEnable ? _uIData.SelectColor : _uIData.DefaultColor;
            _soundImage.color = _musicProgress.SFXEnable ? _uIData.SelectColor : _uIData.DefaultColor;
        }
    }
}