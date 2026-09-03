using UnityEngine;

namespace Assets.Scripts.Services
{
    public class CleanupService
    {
        private readonly OffScreenContainer _offScreenContainer;
        private readonly GameObserver _gameObserver;
        private readonly TimerService _timerService;

        public CleanupService(OffScreenContainer offScreenContainer,GameObserver gameObserver,TimerService timerService)
        {
            _offScreenContainer = offScreenContainer;
            _gameObserver = gameObserver;
            _timerService = timerService;
        }

        public void Cleanup()
        {
            _gameObserver.Cleanup();
            _offScreenContainer.Cleanup();
            _timerService.Stop();
        }
    }
}