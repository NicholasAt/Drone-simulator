using Assets.Scripts.Pool;
using Assets.Scripts.Services.ChunkLoad;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class CleanupService
    {
        private readonly OffScreenContainer _offScreenContainer;
        private readonly GameObserver _gameObserver;
        private readonly TimerService _timerService;
        private readonly VehiclesDestroyPool _vehiclesDestroyPool;
        private readonly ChunkLoaderService _chunkLoaderService;
        private readonly MusicService _musicService;

        public CleanupService(OffScreenContainer offScreenContainer,GameObserver gameObserver,TimerService timerService, VehiclesDestroyPool vehiclesDestroyPool, ChunkLoaderService chunkLoaderService,MusicService musicService)
        {
            _offScreenContainer = offScreenContainer;
            _gameObserver = gameObserver;
            _timerService = timerService;
            _vehiclesDestroyPool = vehiclesDestroyPool;
            _chunkLoaderService = chunkLoaderService;
            _musicService = musicService;
        }

        public void Cleanup()
        {
            _gameObserver.Cleanup();
            _offScreenContainer.Cleanup();
            _vehiclesDestroyPool.Cleanup();
            _timerService.Stop();
            _chunkLoaderService.Stop();
            _musicService.Stop();
        }
    }
}