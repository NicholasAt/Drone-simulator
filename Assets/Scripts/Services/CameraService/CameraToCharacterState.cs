using Assets.Scripts.Character;
using Assets.Scripts.VehicleCamera;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace Assets.Scripts.Services.CameraService
{
    public class CameraToCharacterState : ICameraEnter
    {
        private readonly CharacterComponentsKeeperService _componentsKeeper;
        private readonly GameFactory _gameFactory;
        private Transform _mainCamera;
        private GameObject _cinema;

        public CameraToCharacterState(CharacterComponentsKeeperService componentsKeeper, GameFactory gameFactory)
        {
            _componentsKeeper = componentsKeeper;
            _gameFactory = gameFactory;
        }

        public async UniTask Prepare()
        {
            _mainCamera = Camera.main.transform;
            _cinema = _gameFactory.CinemaCamera;
            await UniTask.CompletedTask;
        }
        public async UniTask Enter()
        {
            IVehicleCamera characterCamera = _componentsKeeper.VehicleCamera;
            if (characterCamera.IsFirstPerson)
            {
                _cinema.SetActive(false);
                _mainCamera.SetParent(characterCamera.Root);
                _mainCamera.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
            else
            {
                if (_cinema.activeInHierarchy)
                    _cinema.SetActive(false);
                await UniTask.DelayFrame(1);

                _cinema.GetComponent<CinemachineCamera>().Follow = characterCamera.Root;
                _cinema.SetActive(true);
            }

            await UniTask.CompletedTask;
        }

        public async UniTask Exit()
        {
            await UniTask.CompletedTask;
        }
    }
}