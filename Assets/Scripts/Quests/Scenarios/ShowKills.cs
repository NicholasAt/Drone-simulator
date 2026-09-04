using Assets.Scripts.ObjecstName;
using Assets.Scripts.UI.Windows.Popup;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Quests.Scenarios
{
    public class ShowKills
    {
        private readonly Dictionary<string, int> _targets = new();
        private readonly StringBuilder _sb = new();
        private PopupMessage _popup;

        public void Init(PopupMessage popup)
        {
            _popup = popup;
        }
        public void Run(IList<GameObject> targets)
        {
            _sb.Clear();
            _targets.Clear();

            foreach (GameObject target in targets)
            {
                if (target.TryGetComponent(out IObjectName objectName))
                {
                    string key = objectName.GetName();

                    if (_targets.ContainsKey(key) == false)
                        _targets.Add(key, 0);

                    _targets[key]++;
                }
            }
            foreach (KeyValuePair<string, int> item in _targets)
                _sb.AppendLine($"x{item.Value} {item.Key}");

            _popup.Show(_sb.ToString());
        }
    }
}