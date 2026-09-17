using Assets.Scripts.Data;
using Assets.Scripts.Services;
using Assets.Scripts.Services.GameProgress;
using Assets.Scripts.Services.GameStates;
using Assets.Scripts.Services.InputService;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Infrastructure
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] private GameObject[] _dontDestroyOnLoad;
        private GameStateMachine _stateMachine;
        private IInputService _inputService;
        private ChunkData _chunkData;
        private ProgressService _progressService;
        private MusicService _audioMixerService;
        private SceneLoader _sceneLoader;
        private GameData _gameData;

        [Inject]
        private void Constuct(GameStateMachine stateMachine, IInputService inputService, ChunkData chunkData, ProgressService progressService, MusicService audioMixerService, SceneLoader sceneLoader, GameData gameData)
        {
            _stateMachine = stateMachine;
            _inputService = inputService;
            _chunkData = chunkData;
            _progressService = progressService;
            _audioMixerService = audioMixerService;
            _sceneLoader = sceneLoader;
            _gameData = gameData;
        }

        public async UniTask Run()
        {
            _inputService.Init();
            _chunkData.Init();
            foreach (GameObject obj in _dontDestroyOnLoad)
            {
                obj.transform.parent = null;
                DontDestroyOnLoad(obj);
            }
            _progressService.LoadOrNew();
            await _audioMixerService.Init();
            await _sceneLoader.Init();
            Application.targetFrameRate = _gameData.FrameRate;

            await _stateMachine.LoadMainMenu();
        }
    }
}