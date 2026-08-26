using UnityEngine;

namespace Assets.Scripts.Character
{
    public class CharacterMarker : MonoBehaviour
    {
        [field: SerializeField] public bool IsBot { get; private set; }
    }
}