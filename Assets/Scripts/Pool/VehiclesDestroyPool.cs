using Assets.Scripts.Data.DestroyVehiclesEffect;
using Assets.Scripts.Effects.Vehicles;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Pool
{
    public class VehiclesDestroyPool
    {
        private readonly Dictionary<DestroyEffectId, Queue<IVehiclesDestroyEffect>> _pool = new();
        private readonly GameFactory _gameFactory;

        public VehiclesDestroyPool(GameFactory gameFactory)
        {
            _gameFactory = gameFactory;
        }

        public void Cleanup()
        {
            _pool.Clear();
        }

        public async UniTask<IVehiclesDestroyEffect> Get(DestroyEffectId id, CancellationToken ct)
        {
            if (_pool.TryGetValue(id, out Queue<IVehiclesDestroyEffect> objecs) && objecs.Count > 0)
            {
                return objecs.Dequeue();
            }

            IVehiclesDestroyEffect instance = await _gameFactory.CreateVehiclesDestroyEffect(id, ct);
            instance.OnHide += () => ReturnPool(id, instance);
           
            return instance;
        }

        private void ReturnPool(DestroyEffectId id, IVehiclesDestroyEffect effect)
        {
            if (!_pool.TryGetValue(id, out Queue<IVehiclesDestroyEffect> queue))
            {
                queue = new Queue<IVehiclesDestroyEffect>();
                _pool.Add(id, queue);
            }
            queue.Enqueue(effect);
        }
    }
}