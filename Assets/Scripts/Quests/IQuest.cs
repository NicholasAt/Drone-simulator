using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Quests
{
    public interface IQuest
    {
        UniTask Run();
    }
}