using Assets.Scripts.Logic;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Bots
{
    public class BotHealth : MonoBehaviour, IApplyDamage
    {
        public Action Happened { get; set; }
        bool _isDead;
        public void Hit(float damage)
        {
            if (_isDead)
                return;

            _isDead = true;
            const float TempPos = 5;
            const float TempSpeed = 15;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.DOMove(transform.position + Vector3.down * TempPos, TempSpeed).OnComplete(() => gameObject.SetActive(false));
            Happened?.Invoke();
        }
    }
}