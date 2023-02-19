using System.Collections.Generic;

namespace CDOverhaul
{
    public static class EnableCursorController
    {
        private static readonly List<byte> _conditionIDs = new List<byte>();
        private static byte _nextId = 1;

        internal static void Reset()
        {
            _conditionIDs.Clear();
        }

        /// <summary>
        /// Make the game understand that cursor should be enabled until you remove condition (<see cref="EnableCursorController.RemoveCondition(in int)"/>) the ID which we got from this method
        /// </summary>
        /// <returns></returns>
        public static byte AddCondition()
        {
            byte number = _nextId;
            _nextId++;
            if (_nextId >= 255)
            {
                _nextId = 1;
            }

            _conditionIDs.Add(number);

            GameUIRoot.Instance.RefreshCursorEnabled();

            return number;
        }

        /// <summary>
        /// Remove condition with ID
        /// </summary>
        /// <param name="id"></param>
        public static void RemoveCondition(in byte id)
        {
            if (_conditionIDs.Contains(id))
            {
                _ = _conditionIDs.Remove(id);
                GameUIRoot.Instance.RefreshCursorEnabled();

                if (_conditionIDs.Count == 0)
                {
                    _nextId = 1;
                }
            }
        }

        public static bool HasToEnableCursor()
        {
            return _conditionIDs.Count != 0;
        }
    }
}
