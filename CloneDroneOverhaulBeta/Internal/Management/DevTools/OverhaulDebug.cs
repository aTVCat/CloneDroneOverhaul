using System;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulDebug
    {
        public const string PREFIX = "[OVERHAUL] ";

        public static void Log(object @object, EDebugType type)
        {
            Debug.Log(getLogString(@object, type));
        }

        public static void Warn(object @object, EDebugType type)
        {
            Debug.LogWarning(getLogString(@object, type));
        }

        public static void Error(object @object, EDebugType type)
        {
            Debug.LogError(getLogString(@object, type));
        }

        private static string getLogString(object @object, EDebugType type)
        {
            DateTime time = DateTime.Now;
            string timeString = time.Hour + ":" + time.Minute + ":" + time.Second + ":" + time.Millisecond;
            return PREFIX + getDebugTypeString(type) + @object.ToString() + ", " + timeString;
        }

        private static string getDebugTypeString(EDebugType type)
        {
            return string.Format("[{0}] ", type);
        }
    }
}
