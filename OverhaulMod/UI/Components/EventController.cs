using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.UI
{
    public class EventController : MonoBehaviour
    {
        private List<KeyValuePair<string, Action>> m_addedEventListeners;

        public void AddEventListener(string eventName, Action action)
        {
            List<KeyValuePair<string, Action>> list = m_addedEventListeners;
            if (list == null)
            {
                list = new List<KeyValuePair<string, Action>>();
                m_addedEventListeners = list;
            }

            GlobalEventManager.Instance.AddEventListener(eventName, action);
            list.Add(new KeyValuePair<string, Action>(eventName, action));
        }

        private void OnDestroy()
        {
            List<KeyValuePair<string, Action>> list = m_addedEventListeners;
            if (list == null || list.Count == 0)
                return;

            foreach (KeyValuePair<string, Action> kv in list)
            {
                GlobalEventManager.Instance.RemoveEventListener(kv.Key, kv.Value);
            }
        }
    }
}
