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

        /// <summary>
        /// Unused
        /// </summary>
        public WeaponVariant Variant
        {
            get;
            set;
        }

        public WeaponSkinModel(GameObject model, WeaponSkinModelOffset offset)
        {
            Model = model;
            Offset = offset;
        }

        /// <summary>
        /// Currently unused
        /// </summary>
        /// <param name="fire"></param>
        /// <param name="multiplayer"></param>
        private void setVariant(bool fire, bool multiplayer)
        {
            Variant = WeaponVariant.None;
            if (!fire && !multiplayer)
            {
                Variant = WeaponVariant.Default;
            }
            else if (!fire && multiplayer)
            {
                Variant = WeaponVariant.DefaultMultiplayer;
            }
            else if (fire && !multiplayer)
            {
                Variant = WeaponVariant.Fire;
            }
            else if (fire && multiplayer)
            {
                Variant = WeaponVariant.FireMultiplayer;
            }
        }
    }
}
