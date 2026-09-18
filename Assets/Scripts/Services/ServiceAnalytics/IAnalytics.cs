using Assets.Scripts.Data.Quests;

namespace Assets.Scripts.Services.ServiceAnalytics
{
    public interface IAnalytics
    {
        void LeaveMission(QuestID quest, string time);
        void LoseMission(QuestID quest, string time);
        void SelectLevelEvent(string transport, QuestID questID);
        void WinMission(QuestID quest, string time, int stars);
    }
}