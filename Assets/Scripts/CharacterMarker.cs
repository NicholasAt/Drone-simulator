using UnityEngine;

namespace Assets.Scripts
{
    public class CharacterMarker : MonoBehaviour
    {
        [field: SerializeField] public bool IsBot { get; private set; }
    }
}