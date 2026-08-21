using Assets.Scripts.Data.Quests;

namespace Assets.Scripts.Services.GameProgress
{
    public class TempLevelProgress
    {
        public QuestID QuestID { get; private set; }
        public void SetQuestId(QuestID questID)
        {
            QuestID = questID;
        }
    }
}