using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Quests.Scenarios
{
    public abstract class BaseQuest : MonoBehaviour, IQuest
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