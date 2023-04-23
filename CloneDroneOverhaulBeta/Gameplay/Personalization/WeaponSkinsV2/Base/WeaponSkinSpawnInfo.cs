using UnityEngine;

namespace CDOverhaul.Gameplay
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

        public bool IsReparented
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