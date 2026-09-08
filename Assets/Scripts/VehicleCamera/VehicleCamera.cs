using UnityEngine;

namespace Assets.Scripts.VehicleCamera
{
    public class VehicleCamera : MonoBehaviour, IVehicleCamera
    {
        [field: SerializeField] public Transform Root { get; private set; }
        [field: SerializeField] public bool IsFirstPerson { get; private set; }
        private void OnDrawGizmos()
        {
            if (Root != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(Root.position, 0.5f);
            }
        }
    }
}