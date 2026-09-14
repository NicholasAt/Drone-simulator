using Assets.Scripts.Character;
using Assets.Scripts.Data.DestroyVehiclesEffect;
using Assets.Scripts.Pool;
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
        private DestroyEffectId _id;
        private VehiclesDestroyPool _vehiclesPool;
        private CharacterComponentsKeeperService _characterComponentsKeeper;

        [Inject]
        private void Construct(VehiclesDestroyPool vehiclesDestroyPool, CharacterComponentsKeeperService characterComponentsKeeperService)
        {
            _vehiclesPool = vehiclesDestroyPool;
            _characterComponentsKeeper = characterComponentsKeeperService;
        }

        public void Init(DestroyEffectId id)
        {
            _id = id;
            IgnoreColliders();
        }

        public async UniTask Play()
        {
            IVehiclesDestroyEffect effect = await _vehiclesPool.Get(_id, this.GetCancellationTokenOnDestroy());
            effect.Show(transform.position + Vector3.up, Vector3.up);
        }
        private void IgnoreColliders()
        {
            
        }
    }
}