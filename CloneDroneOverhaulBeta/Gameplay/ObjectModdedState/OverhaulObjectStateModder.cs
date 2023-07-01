using System.Collections.Generic;

namespace CDOverhaul.Patches
{
    public static class OverhaulObjectStateModder
    {
        private static readonly Dictionary<int, Dictionary<string, object>> m_ObjectStates = new Dictionary<int, Dictionary<string, object>>();
        private static readonly Dictionary<int, UnityEngine.Object> m_CachedObjects = new Dictionary<int, UnityEngine.Object>();

        public static void SetObjectState(this UnityEngine.Object theObject, string paramName, object value)
        {
            if (!theObject)
                return;

            int instanceID = theObject.GetInstanceID();
            createEntryIfRequired(theObject, instanceID);

            Dictionary<string, object> dictionary = m_ObjectStates[instanceID];
            if (!dictionary.ContainsKey(paramName))
            {
                dictionary.Add(paramName, value);
                return;
            }

            dictionary[paramName] = value;
        }

        public static T GetObjectState<T>(this UnityEngine.Object theObject, string paramName)
        {
            if (!theObject || string.IsNullOrEmpty(paramName))
                return default;

            int instanceID = theObject.GetInstanceID();
            if (!m_ObjectStates.ContainsKey(instanceID))
                return default;

            Dictionary<string, object> dictionary = m_ObjectStates[instanceID];
            return !dictionary.ContainsKey(paramName) ? default : (T)dictionary[paramName];
        }

        public static void Reset()
        {
            List<int> ids = new List<int>();
            foreach (int id in m_CachedObjects.Keys)
            {
                UnityEngine.Object obj = m_CachedObjects[id];
                if (!obj)
                {
                    ids.Add(id);
                }
            }

            if (ids.IsNullOrEmpty())
                return;

            int i = 0;
            do
            {
                _ = m_CachedObjects.Remove(ids[i]);
                _ = m_ObjectStates.Remove(ids[i]);
                i++;
            } while (i < ids.Count);
        }

        private static void createEntryIfRequired(UnityEngine.Object theObject, int instanceID)
        {
            if (m_CachedObjects.ContainsKey(instanceID))
                return;

            m_ObjectStates.Add(instanceID, new Dictionary<string, object>());
            m_CachedObjects.Add(instanceID, theObject);
        }
    }
}
