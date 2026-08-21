namespace Assets.Scripts.Services.GameProgress
{
    public class ProgressService
    {
        public readonly TempLevelProgress TempLevelProgress;
        public ProgressService()
        {
            TempLevelProgress = new();
        }
    }
}