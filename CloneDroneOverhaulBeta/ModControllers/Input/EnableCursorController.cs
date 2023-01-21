using System.Collections.Generic;

namespace CDOverhaul
{
    public static class EnableCursorController
    {
        private static List<int> _conditionIDs = new List<int>();

        internal static void Reset()
        {
            _conditionIDs.Clear();
        }

        public static int AddCondition()
        {
            int number = UnityEngine.Random.Range(0, 999999);
            _conditionIDs.Add(number);

            GameUIRoot.Instance.RefreshCursorEnabled();

            return number;
        }

        public static void RemoveCondition(in int id)
        {
            if (_conditionIDs.Contains(id))
            {
                _conditionIDs.Remove(id);
                GameUIRoot.Instance.RefreshCursorEnabled();
            }
        }

        public static bool HasToEnableCursor()
        {
            return _conditionIDs.Count != 0;
        }
    }
}
