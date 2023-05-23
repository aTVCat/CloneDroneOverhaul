using UnityEngine;

namespace CDOverhaul
{
    internal readonly struct ExclusiveColorInfo
    {
        public readonly Color NewColor;
        public readonly int ColorToReplace;

        public ExclusiveColorInfo(Color favColor, int replaceColorIndex)
        {
            NewColor = favColor;
            ColorToReplace = replaceColorIndex;
        }
    }
}
