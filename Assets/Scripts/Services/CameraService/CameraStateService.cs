using Assets.Scripts.Data.CameraAnimationData;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Services.CameraService
{
    public interface ICameraEnterParam3<T1, T2, T3> : ICameraExit
    {
        UniTask Enter(T1 t1, T2 t2, T3 t3);
    }

    public interface ICameraEnter : ICameraExit
    {
        UniTask Enter();
    }

    public interface ICameraExit
    {
        UniTask Prepare();
        UniTask Exit();
    }

    public class CameraStateService
    {
        private readonly Dictionary<Type, ICameraExit> _states;
        private ICameraExit _activeState;
        public CameraStateService(CameraData cameraData, GameObserver gameObserver, TransportFactory transportFactory)
        {
            _states = new Dictionary<Type, ICameraExit>()
            {
                [typeof(CameraAnimationState)] = new CameraAnimationState(gameObserver, cameraData),
                [typeof(CameraToCharacterState)] = new CameraToCharacterState(transportFactory)
            };
        }
        public async UniTask Prepare()
        {
            foreach (ICameraExit state in _states.Values)
            {
                await state.Prepare();
            }
        }
        public async UniTask Enter<TState, T1, T2, T3>(T1 t1, T2 t2, T3 t3) where TState : class, ICameraEnterParam3<T1, T2, T3>
        {
            TState state = ChangeState<TState>();
            await state.Enter(t1, t2, t3);
        }

        public async UniTask Enter<TState>() where TState : class, ICameraEnter
        {
            TState state = ChangeState<TState>();
            await state.Enter();
        }

        private TState ChangeState<TState>() where TState : class, ICameraExit
        {
            _activeState?.Exit();
            TState state = _states[typeof(TState)] as TState;
            _activeState = state;
            return state;
        }
    }
}