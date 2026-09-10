using UnityEngine;

namespace Assets.Scripts.Services.GameProgress
{
    public class SaveProgress<TValue>
    {
        private TValue _value;
        private string _key;
        private bool _init;

        public void BaseInit(TValue value, string key)
        {
            _value = value;
            _key = key;
            _init = true;
        }

        public void BaseSave()
        {
            if (_init == false)
                Debug.LogError("no init");

            Debug.Log("saved");
            PlayerPrefs.SetString(_key, JsonUtility.ToJson(_value));
            PlayerPrefs.Save();
        }

        public bool BaseTryLoad(out TValue value)
        {
            if (_init == false)
                Debug.LogError("no init");

            string json = PlayerPrefs.GetString(_key);
            if (string.IsNullOrEmpty(json) == false)
            {
                value = JsonUtility.FromJson<TValue>(json);
                return value != null;
            }
            value = default;
            return false;
        }
    }
}