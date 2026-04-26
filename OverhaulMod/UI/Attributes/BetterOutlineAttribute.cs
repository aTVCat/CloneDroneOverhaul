using System;
using UnityEngine;

namespace OverhaulMod.UI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BetterOutlineAttribute : Attribute
    {
        public Vector2 Distance;

        public Color Color;

        public bool ReplaceOriginalOutline;

        public BetterOutlineAttribute(Vector2 distance, Color color)
        {
            ReplaceOriginalOutline = false;
            Distance = distance;
            Color = color;
        }

        public BetterOutlineAttribute(float distance, Color color)
        {
            ReplaceOriginalOutline = false;
            Distance = Vector2.one * distance;
            Color = color;
        }

        public BetterOutlineAttribute()
        {
            ReplaceOriginalOutline = true;
        }
    }
}