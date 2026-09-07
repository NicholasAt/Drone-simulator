using Assets.Scripts.Data.DestroyVehiclesEffect;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Effects.Vehicles
{
    public interface IVehiclesDestroyEffectPlayer
    {
        UniTask Play();
    }

    public class VehiclesDestroyEffectPlayer : MonoBehaviour, IVehiclesDestroyEffectPlayer
    {
        private GameFactory _gameFactory;
        private DestroyEffectId _id;

        [Inject]
        private void Construct(GameFactory gameFactory)
        {
            _gameFactory = gameFactory;
        }

        public void Init(DestroyEffectId id)
        {
            _id = id;
        }

        public async UniTask Play()
        {
            IVehiclesDestroyEffect effect = await _gameFactory.CreateVehiclesDestroyEffect(_id, this.GetCancellationTokenOnDestroy());
            effect.Run(transform.position + Vector3.up, Vector3.up);
        }
    }
}