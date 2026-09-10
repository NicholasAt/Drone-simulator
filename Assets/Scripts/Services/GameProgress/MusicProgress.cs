using UnityEngine;

namespace Assets.Scripts.Services.GameProgress
{
    public class MusicProgress : SaveProgress<MusicProgress>
    {
        [field: SerializeField] public bool MusicEnable { get; private set; }
        [field: SerializeField] public bool SoundEnable { get; private set; }
        public MusicProgress()
        {
            BaseInit(this, Constants.Save.MusicKey);
        }

        public void LoadOrNew()
        {
            if (BaseTryLoad(out MusicProgress progress))
            {
                MusicEnable = progress.MusicEnable;
                SoundEnable = progress.SoundEnable;
            }
            else
            {
                MusicEnable = true;
                SoundEnable = true;
            }
        }
        public void ChangeMusic(bool isEnable, bool isSave = true)
        {
            MusicEnable = isEnable;
            if (isSave)
                BaseSave();
        }
        public void ChangeSound(bool isEnable, bool isSave = true)
        {
            SoundEnable = isEnable;
            if (isSave)
                BaseSave();
        }
    }
}