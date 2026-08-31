using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Quests.Scenarios
{
    public abstract class BaseQuest : MonoBehaviour
    {
        public async UniTask Run()
        {
            await OnRun();
        }
        protected virtual async UniTask OnRun()
        {
            await UniTask.CompletedTask;
        }
    }
}