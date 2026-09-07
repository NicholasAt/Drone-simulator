using Assets.Scripts.Pool;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class CleanupService
    {
        private readonly OffScreenContainer _offScreenContainer;
        private readonly GameObserver _gameObserver;
        private readonly TimerService _timerService;
        private readonly VehiclesDestroyPool _vehiclesDestroyPool;

        public CleanupService(OffScreenContainer offScreenContainer,GameObserver gameObserver,TimerService timerService, VehiclesDestroyPool vehiclesDestroyPool)
        {
            _offScreenContainer = offScreenContainer;
            _gameObserver = gameObserver;
            _timerService = timerService;
            _vehiclesDestroyPool = vehiclesDestroyPool;
        }

        public void Cleanup()
        {
            _gameObserver.Cleanup();
            _offScreenContainer.Cleanup();
            _vehiclesDestroyPool.Cleanup();
            _timerService.Stop();
        }
    }
}