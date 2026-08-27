using UnityEngine;

namespace Assets.Scripts.Bots
{
    public class BotRefresher : MonoBehaviour
    {
        private IRefresh[] _refreshes;
        private IRefreshPositions[] _refreshesPositions;

        private void Awake()
        {
            _refreshes = GetComponentsInChildren<IRefresh>();
            _refreshesPositions = GetComponentsInChildren<IRefreshPositions>();
        }
        public void RespawnPosition(Vector3 position, Quaternion rotation)
        {
            foreach (IRefreshPositions refresher in _refreshesPositions)
            {
                refresher.Show(position, rotation);
            }
        }
        public void Refresh()
        {
            foreach (IRefresh refresh in _refreshes)
            {
                refresh.Refresh();
            }
        }
    }
}