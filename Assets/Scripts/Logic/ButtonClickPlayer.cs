using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Logic
{
    public class ButtonClickPlayer : MonoBehaviour
    {
        [SerializeField] private Button[] _buttons;
        private MusicService _musicService;

        [Inject]
        private void Construct(MusicService musicService)
        {
            _musicService = musicService;
        }
        private void Start()
        {
            foreach (Button button in _buttons)
            {
                button.onClick.AddListener(Play);
            }
        }

        private void Play()
        {
            _musicService.Beep();
        }
    }
}