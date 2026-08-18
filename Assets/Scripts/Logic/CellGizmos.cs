using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class CellGizmos : MonoBehaviour
    {
        [SerializeField] private int _posOffset = 250;
        [SerializeField] private float _offsetY = 250;
        [SerializeField] private int _cellCount = 10;
        [SerializeField] private int _cellSize = 500;

        private void OnDrawGizmos()
        {
            if (enabled == false)
                return;

            Gizmos.color = Color.yellow;
            for (int x = 0; x < _cellCount; x++)
            {
                for (int z = 0; z < _cellCount; z++)
                {
                    Vector3 pos = new Vector3(x * _cellSize, _offsetY, z * _cellSize);
                    pos += new Vector3(_posOffset, 0, _posOffset);
                    Gizmos.DrawWireCube(pos, Vector3.one * _cellSize);
                }
            }
        }
    }
}