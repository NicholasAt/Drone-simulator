using Assets.Scripts.Data.DestroyVehiclesEffect;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Data.HelicoptersData
{
    [Serializable]
    public class HelicopterConfig
    {
        [field: SerializeField] public HelicopterID ID { get; private set; }
        [field: SerializeField] public DestroyEffectId DestroyEffect { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject HelicopterReference { get; private set; }

        [field: Header("Controller")]
        [field: SerializeField] public float MoveAxisTime { get; private set; } = 1;
        [field: SerializeField] public float MoveAxisTimeLowing { get; private set; } = 0.3f;
        [field: SerializeField] public float CollectiveTime { get; private set; } = 3;
        [field: SerializeField] public float CollectiveSpeed { get; private set; } = 500;

        [field: Header("Main rotor")]
        [field: SerializeField] public float LiftForce { get; private set; } = 60;
        [field: SerializeField] public float ForwardLiftForce { get; private set; } = 70;
        [field: SerializeField] public float RightLiftForce { get; private set; } = 25;
        [field: SerializeField] public float Gravity { get; private set; } = -500;

        [field: Header("Tail rotor")]
        [field: SerializeField] public float TailForce { get; private set; } = 0.3f;
        [field: SerializeField] public float TailInputSpeed { get; private set; } = 5;

        [field: Header("Drag")]
        [field: SerializeField] public Vector3 LinearDrag { get; private set; } = new Vector3(2, 2, 2);
        [field: SerializeField] public Vector3 AngularDrag { get; private set; } = new(2, 1, 2.5f);

        [field: Header("Balance")]
        [field: SerializeField] public float BalanceForce { get; private set; } = 5;
        [field: SerializeField] public float IdleYDrag { get; private set; } = 8;

        [field: Header("Effect")]
        [field: SerializeField] public Vector2 EffectSpeedRotate { get; private set; } = new(700, 1100);
        [field: SerializeField] public float EffectCollectiveSmoothSpeed { get; private set; } = 1000;
        [field: SerializeField] public AnimationCurve AudioPitchCurve { get; private set; }
    }
}