using System;
using UnityEngine;

namespace Assets.Scripts.Services.GameProgress
{
    public class MusicProgress : SaveProgress<MusicProgress>
    {
        [field: SerializeField] public bool MusicEnable { get; private set; }
        [field: SerializeField] public bool SFXEnable { get; private set; }
        public Action OnChange { get; set; }

        public MusicProgress()
        {
            BaseInit(this, Constants.Save.MusicKey);
        }

        public void LoadOrNew()
        {
            if (BaseTryLoad(out MusicProgress progress))
            {
                MusicEnable = progress.MusicEnable;
                SFXEnable = progress.SFXEnable;
            }
            else
            {
                MusicEnable = true;
                SFXEnable = true;
            }
        }
        public void ChangeMusic(bool isEnable, bool isSave = true)
        {
            MusicEnable = isEnable;
            if (isSave)
                BaseSave();
            OnChange?.Invoke();
        }
        public void ChangeSFX(bool isEnable, bool isSave = true)
        {
            SFXEnable = isEnable;
            if (isSave)
                BaseSave();
            OnChange?.Invoke();
        }
    }
}