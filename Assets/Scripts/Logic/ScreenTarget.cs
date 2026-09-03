using UnityEngine;
using Zenject;

namespace Assets.Scripts.Logic
{
    public class ScreenTarget : MonoBehaviour
    {
        private OffScreenContainer _offScreenContainer;

        [Inject]
        private void Construct(OffScreenContainer offScreenContainer)
        {
            _offScreenContainer = offScreenContainer;
        }

        private void OnEnable()
        {
            _offScreenContainer.Add(transform);
        }

        private void OnDisable()
        {
            _offScreenContainer.Remove(transform);
        }
    }
}