using Assets.Scripts.Data.Quests;
using Unity.Services.Analytics;
using static Assets.Scripts.Constants.AnalyticsConstants;

namespace Assets.Scripts.Services.ServiceAnalytics
{
    public class UnityAnalyticsService : IAnalytics
    {
        public void SelectLevelEvent(string transport, QuestID questID)
        {
            CustomEvent @event = new(SelectMissionEvent)
            {
                { TransportParameter, transport },
                { MissionNameParameter, questID.ToString()}
            };
            AnalyticsService.Instance.RecordEvent(@event);
        }

        public void LeaveMission(QuestID quest, string time)
        {
            CustomEvent @event = new CustomEvent(LeaveMissionEvent)
            {
                { TimeParameter, time},
                { MissionNameParameter, quest.ToString()},
            };
            AnalyticsService.Instance.RecordEvent(@event);
        }

        public void WinMission(QuestID quest, string time, int stars)
        {
            CustomEvent @event = new CustomEvent(WinMissionEvent)
            {
                { TimeParameter, time},
                { MissionNameParameter, quest.ToString()},
                { StarsParameter, stars},
            };
            AnalyticsService.Instance.RecordEvent(@event);
        }

        public void LoseMission(QuestID quest, string time)
        {
            CustomEvent @event = new CustomEvent(LoseMissionEvent)
            {
                { TimeParameter, time},
                { MissionNameParameter, quest.ToString()},
            };
            AnalyticsService.Instance.RecordEvent(@event);
        }
    }
}