using CDOverhaul.Gameplay;
using UnityEngine;

namespace CDOverhaul
{
    public class WeaponSkinSpawnInfo
    {
        public WeaponType Type
        {
            get;
            set;
        }

        public WeaponVariant Variant
        {
            get;
            set;
        }

        public GameObject Model
        {
            get;
            set;
        }

        public void DestroyModel()
        {
            if (Model != null)
            {
                GameObject.Destroy(Model);
            }
        }
    }
}