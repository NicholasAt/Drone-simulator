using Assets.Scripts.Character;
using Assets.Scripts.Data.CameraAnimationData;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Services.CameraService
{
    public class CameraStateService
    {
        private readonly Dictionary<Type, ICameraExit> _states;
        private ICameraExit _activeState;
        public CameraStateService(CameraData cameraData, GameObserver gameObserver, CharacterComponentsKeeperService componentsKeeper, GameFactory gameFactory)
        {
            _states = new Dictionary<Type, ICameraExit>()
            {
                [typeof(CameraAnimationState)] = new CameraAnimationState(gameObserver, cameraData, gameFactory),
                [typeof(CameraToCharacterState)] = new CameraToCharacterState(componentsKeeper, gameFactory)
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