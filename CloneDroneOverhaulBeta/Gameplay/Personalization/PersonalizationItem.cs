using CDOverhaul.Gameplay.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Gameplay
{
    public abstract class PersonalizationItem : OverhaulDisposable
    {
        public const string NO_DESCRIPTION_PROVIDED = "No description provided";
        public const string NO_AUTHOR_SPECIFIED = "N/A";
        public const string NO_NAME_SPECIFIED = "Unknown item";

        public string Name = NO_NAME_SPECIFIED;
        public string Description = NO_DESCRIPTION_PROVIDED;
        public string Author = NO_AUTHOR_SPECIFIED;

        public static FieldInfo[] GetFields<T>() where T : PersonalizationItem
        {
            return typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);
        }
    }
}
