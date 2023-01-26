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

        /// <summary>
        /// Make the game understand that cursor should be enabled until you remove condition (<see cref="EnableCursorController.RemoveCondition(in int)"/>) the ID which we got from this method
        /// </summary>
        /// <returns></returns>
        public static int AddCondition()
        {
            int number = UnityEngine.Random.Range(0, 999999);
            _conditionIDs.Add(number);

            GameUIRoot.Instance.RefreshCursorEnabled();

            return number;
        }

        /// <summary>
        /// Remove condition with ID
        /// </summary>
        /// <param name="id"></param>
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
