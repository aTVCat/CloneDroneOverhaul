using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public abstract class WeaponSkinSpecialBehaviour : OverhaulBehaviour
    {
        public abstract void OnBeginDraw();
        public abstract void OnEndDraw();
    }
}