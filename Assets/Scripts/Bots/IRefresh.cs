using UnityEngine;

namespace Assets.Scripts.Bots
{
    public interface IRefreshPositions
    {
        void Show(Vector3 pos, Quaternion rotate);
        void Hide();
    }
    public interface IRefresh
    {
        void Refresh();
    }
}