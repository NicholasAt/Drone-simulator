namespace Assets.Scripts
{
    public class Constants
    {
        public class SceneConstants
        {
            public const string MenuSceneKey = "MainMenu";
            public const string Location1SceneKey = "Location1";
        }
        public class Save
        {
            public const string StarsKey = "Starskey";
            public const string MusicKey = "Musickey";
        }
        public class MusicSettings
        {
            public const string SFXVolume = "SFXVolume";
            public const string MusicVolume = "MusicVolume";
        }
        public class AnalyticsConstants
        {
            public const string SelectMissionEvent = "select_mission";
            public const string LeaveMissionEvent = "leave_mission";
            public const string WinMissionEvent = "win_mission";
            public const string LoseMissionEvent = "lose_mission";

            public const string TransportParameter = "transport_name";
            public const string MissionNameParameter = "mission_name";
            public const string TimeParameter = "mission_timer";
            public const string StarsParameter = "stars";
        }
    }
}