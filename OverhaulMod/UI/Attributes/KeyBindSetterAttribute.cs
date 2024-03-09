using System;
using UnityEngine;

namespace OverhaulMod.UI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class KeyBindSetterAttribute : Attribute
    {
        public KeyCode DefaultKey;

        public KeyBindSetterAttribute(KeyCode defaultKey)
        {
            DefaultKey = defaultKey;
        }
    }
}
