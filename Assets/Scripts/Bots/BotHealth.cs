using Assets.Scripts.Logic;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Bots
{
    public class BotHealth : MonoBehaviour, IApplyDamage, IRefresh
    {
        public Action Happened { get; set; }
        public bool Died { get; private set; }

        private IRefreshPositions _refreshPositions;
        private NavMeshAgent _agent;

        private void Awake()
        {
            _refreshPositions = GetComponent<IRefreshPositions>();
            _agent = GetComponent<NavMeshAgent>();
        }
        public void Hit(float damage)
        {
            if (Died)
                return;

            Died = true;
            if (_agent != null)
                _agent.enabled = false;
            _refreshPositions?.Hide();
            Happened?.Invoke();
        }

        public void Refresh()
        {
            Died = false;
        }
    }
}