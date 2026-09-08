using Assets.Scripts.Bots;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Character
{
    public class CharacterRefresher : MonoBehaviour, IRefreshPositions
    {
        [SerializeField] private Rigidbody _rb;
        
        public void Show(Vector3 pos, Quaternion rotate)
        {
            transform.SetPositionAndRotation(pos, rotate);
            _rb.MovePosition(pos);
            _rb.MoveRotation(rotate);
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
}