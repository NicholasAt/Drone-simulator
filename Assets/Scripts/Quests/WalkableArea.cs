using UnityEngine;

namespace Assets.Scripts.Quests
{
    public class WalkableArea : MonoBehaviour
    {
        [SerializeField] private Vector3 _size = Vector3.one;
        public void GetArea(out Transform matrix, out Vector3 size)
        {
            matrix = transform;
            size = _size;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Vector3 size = new(_size.x, _size.y, _size.z);
            Gizmos.DrawWireCube(new Vector3(0, _size.y / 2, 0), size);
        }
    }
}