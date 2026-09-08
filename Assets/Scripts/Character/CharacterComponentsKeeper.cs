using Assets.Scripts.Bots;
using Assets.Scripts.Effects.Vehicles;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class CharacterComponentsKeeper
    {
        public GameObject Character { get; }
        public IRefreshPositions CharacterRefresher { get; }
        public CharacterHit CharacterHit { get; }
        public IVehiclesDestroyEffectPlayer DestroyEffectPlayer { get; }

        public CharacterComponentsKeeper(GameObject character, IRefreshPositions refresher, CharacterHit characterHit, IVehiclesDestroyEffectPlayer destroyEffectPlayer)
        {
            Character = character;
            CharacterRefresher = refresher;
            CharacterHit = characterHit;
            DestroyEffectPlayer = destroyEffectPlayer;
        }
        public Vector3 Pos()
        {
            return Character.transform.position;
        }
    }
}