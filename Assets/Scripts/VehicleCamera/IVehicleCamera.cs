using UnityEngine;

namespace Assets.Scripts.VehicleCamera
{
    public interface IVehicleCamera
    {
        Transform Root { get; }
        bool IsFirstPerson { get; }
    }
}