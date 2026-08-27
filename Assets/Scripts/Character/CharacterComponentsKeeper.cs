using Assets.Scripts.Bots;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class CharacterComponentsKeeper
    {
        public GameObject Character { get; }
        public IRefreshPositions CharacterRefresher { get; }
        public CharacterHit CharacterHit { get; }
        public CharacterComponentsKeeper(GameObject character, IRefreshPositions refresher, CharacterHit characterHit)
        {
            Character = character;
            CharacterRefresher = refresher;
            CharacterHit = characterHit;
        }
        public Vector3 Pos()
        {
            return Character.transform.position;
        }
    }
}