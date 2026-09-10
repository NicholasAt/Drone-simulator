using Assets.Scripts.Data.Quests;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Services.GameProgress
{
    [Serializable]
    public class StartsProgress : SaveProgress<StartsProgress>
    {
        [Serializable]
        private class StarsData
        {
            public QuestID Id;
            public int Stars;
        }

        [SerializeField] private List<StarsData> _iDs = new();

        public StartsProgress()
        {
            BaseInit(this, Constants.Save.StarsKey);
        }
        public void LoadOrNew()
        {
            if (BaseTryLoad(out StartsProgress value))
            {
                foreach (StarsData data in value._iDs)
                    _iDs.Add(data);
            }
        }

        public void ProtectedSetAndSave(QuestID id, int stars, bool isSave = true)
        {
            if (stars < 1)
                return;

            if (TryGetData(id, out StarsData data))
            {
                if (stars > data.Stars)
                {
                    Set(id, stars);
                    if (isSave)
                        BaseSave();
                }
            }
            else
            {
                Set(id, stars);
                if (isSave)
                    BaseSave();
            }
        }
        public int GetStars(QuestID id)
        {
            return TryGetData(id, out StarsData data) ? data.Stars : 0;
        }
        private bool TryGetData(QuestID id, out StarsData starsData)
        {
            foreach (StarsData data in _iDs)
            {
                if (data.Id == id)
                {
                    starsData = data;
                    return true;
                }
            }
            starsData = null;
            return false;
        }

        private void Set(QuestID id, int stars)
        {
            if (TryGetData(id, out StarsData data))
            {
                data.Stars = stars;
            }
            else
            {
                _iDs.Add(new StarsData { Id = id, Stars = stars });
            }
        }
    }
}