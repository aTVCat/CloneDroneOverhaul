using UnityEngine;

namespace OverhaulMod.Engine
{
    public class ColorPairFloat
    {
        public Color ColorA, ColorB;

        public ColorPairFloat() { }

        public ColorPairFloat(Color a, Color b)
        {
            ColorA = a;
            ColorB = b;
        }

        public override string ToString()
        {
            return $"{ColorA}-{ColorB}";
        }
    }
}
