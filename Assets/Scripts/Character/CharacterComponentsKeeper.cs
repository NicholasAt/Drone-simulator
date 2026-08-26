using UnityEngine;

namespace Assets.Scripts.Character
{
    public class CharacterComponentsKeeper
    {
        public GameObject Character { get; }
        public CharacterRefresher CharacterRefresher { get; }
        public CharacterHit CharacterHit { get; }
        public CharacterComponentsKeeper(GameObject character, CharacterRefresher characterRefresher, CharacterHit characterHit)
        {
            Character = character;
            CharacterRefresher = characterRefresher;
            CharacterHit = characterHit;
        }
        public Vector3 Pos()
        {
            return Character.transform.position;
        }
    }
}