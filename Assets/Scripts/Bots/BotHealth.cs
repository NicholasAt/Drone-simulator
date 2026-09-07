using Assets.Scripts.Effects.Vehicles;
using Assets.Scripts.Logic;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Bots
{
    public class BotHealth : MonoBehaviour, IApplyDamage, IRefresh
    {
        [SerializeField] private bool _disable;
        public Action Happened { get; set; }
        public bool Died { get; private set; }

        private IRefreshPositions _refreshPositions;
        private NavMeshAgent _agent;
        private IVehiclesDestroyEffectPlayer _effectPlayer;

        private void Awake()
        {
            _refreshPositions = GetComponent<IRefreshPositions>();
            _agent = GetComponent<NavMeshAgent>();
            _effectPlayer = GetComponent<IVehiclesDestroyEffectPlayer>();
        }

        public void Hit(float damage)
        {
            if (Died)
                return;

            Died = true;
            if (_agent != null)
                _agent.enabled = false;
            _refreshPositions?.Hide();

            if (_disable)
                gameObject.SetActive(false);

            if (_effectPlayer != null)
                _effectPlayer.Play().Forget();
            Happened?.Invoke();
        }

        public void Refresh()
        {
            Died = false;
        }
    }
}