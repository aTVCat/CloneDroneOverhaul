using System;
using UnityEngine;

namespace CDOverhaul.Shared
{
    /// <summary>
    /// TO DO
    /// </summary>
    public class ObjectStateListener : MonoBehaviour
    {
        private Action OnDestroyTrigger;

        public static ObjectStateListener AddStateListener(in GameObject gameObject)
        {
            ObjectStateListener result = gameObject.GetComponent<ObjectStateListener>();
            if (result != null)
            {
                return result;
            }

            result = gameObject.AddComponent<ObjectStateListener>();
            return result;
        }

        public void AddOnDestroyTrigger(in Action action)
        {
            OnDestroyTrigger = action;
        }

        private void OnDestroy()
        {
            if (OnDestroyTrigger != null)
            {
                OnDestroyTrigger();
            }
        }
    }
}