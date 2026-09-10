namespace Assets.Scripts.Services.GameProgress
{
    public class ProgressService
    {
        public readonly TempLevelProgress TempLevelProgress;
        public readonly StartsProgress StartsProgress;
        public ProgressService()
        {
            TempLevelProgress = new();
            StartsProgress = new();
        }
        public void LoadOrNew()
        {
            StartsProgress.LoadOrNew();
        }
    }
}