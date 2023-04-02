using OverhaulAPI;
using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class VFXWeaponSkinSwitch : PooledPrefabInstanceBase
    {
        protected override float LifeTime()
        {
            return 5f;
        }
    }
}