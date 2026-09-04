using UnityEngine;

namespace Assets.Scripts.ObjecstName
{
    public class ObjectName : MonoBehaviour, IObjectName
    {
        private string _name;

        public string GetName()
        {
            return _name;
        }

        public void SetName(string name)
        {
            _name = name;
        }
    }
}