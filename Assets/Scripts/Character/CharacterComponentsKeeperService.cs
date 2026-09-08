using Assets.Scripts.Bots;
using Assets.Scripts.Effects.Vehicles;
using Assets.Scripts.VehicleCamera;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class CharacterComponentsKeeperService
    {
        public GameObject Character { get; private set; }
        public IRefreshPositions CharacterRefresher { get; private set; }
        public CharacterHit CharacterHit { get; private set; }
        public IVehiclesDestroyEffectPlayer DestroyEffectPlayer { get; private set; }
        public IVehicleCamera VehicleCamera { get; private set; }

        public void Init(GameObject character, IRefreshPositions refresher, CharacterHit characterHit, IVehiclesDestroyEffectPlayer destroyEffectPlayer, IVehicleCamera vehicleCamera)
        {
            Character = character;
            CharacterRefresher = refresher;
            CharacterHit = characterHit;
            DestroyEffectPlayer = destroyEffectPlayer;
            VehicleCamera = vehicleCamera;
        }
        public Vector3 Pos()
        {
            return Character.transform.position;
        }
    }
}