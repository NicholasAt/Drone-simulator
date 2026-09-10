namespace Assets.Scripts.Services.GameProgress
{
    public class ProgressService
    {
        public readonly TempLevelProgress TempLevelProgress;
        public readonly StartsProgress StartsProgress;
        public readonly MusicProgress MusicProgress;
        public ProgressService()
        {
            TempLevelProgress = new();
            StartsProgress = new();
            MusicProgress = new();
        }
        public void LoadOrNew()
        {
            StartsProgress.LoadOrNew();
            MusicProgress.LoadOrNew();
        }
    }
}