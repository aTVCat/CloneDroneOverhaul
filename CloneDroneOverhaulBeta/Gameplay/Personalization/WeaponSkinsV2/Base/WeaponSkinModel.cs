using OverhaulAPI;
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

        public ModelOffset Offset
        {
            get;
            set;
        }

        public WeaponSkinModel(GameObject model, ModelOffset offset)
        {
            Model = model;
            Offset = offset;
        }
    }
}
