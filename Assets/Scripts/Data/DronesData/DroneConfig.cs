using Assets.Scripts.Data.DestroyVehiclesEffect;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Data.DronesData
{
    [Serializable]
    public class DroneConfig
    {
        [field: SerializeField] public DroneID DroneID { get; private set; }
        [field: SerializeField] public DestroyEffectId DestroyEffect { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject DroneReference { get; private set; }

        [field: Header("Move")]
        [field: SerializeField] public float PitchForce { get; private set; } = 4;
        [field: SerializeField] public float YawForce { get; private set; } = 15;
        [field: SerializeField] public float Force { get; private set; } = 140;
        [field: SerializeField] public float Gravity { get; private set; } = -50;
        [field: SerializeField] public Vector3 Drag { get; private set; } = new(0.7f, 2, 0.7f);
        [field: SerializeField] public Vector3 AngularDrag { get; private set; } = new(2, 1.5f, 2);

        [field: Header("Effect")]
        [field: SerializeField] public Vector2 MinMaxAudio { get; private set; } = new(0.5f, 0.9f);
        [field: SerializeField] public float SpeedUpAudio { get; private set; } = 3;
        [field: SerializeField] public float SpeedDownAudio { get; private set; } = 1;
        [field: SerializeField] public Vector2 EffectSpeedRotate { get; private set; } = new(700, 2100);
    }
}