using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Services.CameraService
{
    public class CameraToCharacterState : ICameraEnter
    {
        private readonly TransportFactory _transportFactory;
        private Transform _mainCamera;

        public CameraToCharacterState(TransportFactory transportFactory)
        {
            _transportFactory = transportFactory;
        }

        public async UniTask Prepare()
        {
            _mainCamera = Camera.main.transform;
            await UniTask.CompletedTask;
        }

        public async UniTask Enter()
        {
            Transform character = _transportFactory.PlayerKeeper.Character.transform;
            _mainCamera.SetParent(character);
            _mainCamera.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            await UniTask.CompletedTask;
        }

        public async UniTask Exit()
        {
            await UniTask.CompletedTask;
        }
    }
}