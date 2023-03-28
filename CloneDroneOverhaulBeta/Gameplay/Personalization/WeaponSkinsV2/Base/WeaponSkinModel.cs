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

        public GameObject Model2
        {
            get;
            private set;
        }

        public GameObject Model3
        {
            get;
            private set;
        }

        public GameObject Model4
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

        public GameObject GetModelVariant(byte variant)
        {
            switch (variant)
            {
                case 0:
                    return Model;
                case 1:
                    return Model2;
                case 2:
                    return Model3;
                case 3:
                    return Model4;
            }
            return null;
        }

        public void SetModelVariant(GameObject prefab, byte variant)
        {
            switch (variant)
            {
                case 0:
                    Model = prefab;
                    break;
                case 1:
                    Model2 = prefab;
                    break;
                case 2:
                    Model3 = prefab;
                    break;
                case 3:
                    Model4 = prefab;
                    break;
            }
        }
    }
}
