using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class WeaponSkinModel
    {
        public GameObject Model
        {
            get;
            private set;
        }

        public WeaponSkinModelOffset Offset
        {
            get;
            set;
        }

        public WeaponSkinModel(GameObject model, WeaponSkinModelOffset offset)
        {
            Model = model;
            Offset = offset;
        }
    }
}
