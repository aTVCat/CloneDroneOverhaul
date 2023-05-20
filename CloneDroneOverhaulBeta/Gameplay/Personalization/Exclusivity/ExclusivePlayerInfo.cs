using UnityEngine;

namespace CDOverhaul
{
    internal readonly struct ExclusivePlayerInfo
    {
        public readonly string Name;
        public readonly string Roles;
        public readonly Color FavColor;
        public readonly int ReplaceColorIndex;
        public readonly string UnlockedSkins;

        public ExclusivePlayerInfo(string name, string roles, Color favColor, int replaceColorIndex, string unlockedSkins = null)
        {
            Name = name;
            Roles = roles;
            FavColor = favColor;
            ReplaceColorIndex = replaceColorIndex;
            UnlockedSkins = unlockedSkins;
        }
    }
}
