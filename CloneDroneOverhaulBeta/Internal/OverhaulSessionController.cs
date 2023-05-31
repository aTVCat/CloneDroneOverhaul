using System.Collections.Generic;
using System.Diagnostics;

namespace CDOverhaul
{
    /// <summary>
    /// Used to store static values
    /// </summary>
    internal static class OverhaulSessionController
    {
        private static readonly Dictionary<string, object> m_Dictionary = new Dictionary<string, object>();

        public static void SetKey(string key, object value)
        {
            StackFrame frame = new StackFrame(1);
            string str = frame.GetMethod().DeclaringType.ToString() + "." + key;

            if (m_Dictionary.ContainsKey(str))
                m_Dictionary[str] = value;
            else
                m_Dictionary.Add(str, value);
        }

        public static T GetKey<T>(string key)
        {
            StackFrame frame = new StackFrame(1);
            string str = frame.GetMethod().DeclaringType.ToString() + "." + key;
            return !m_Dictionary.ContainsKey(str) ? default : (T)m_Dictionary[str];
        }
    }
}
