using OverhaulAPI;
using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    /// <summary>
    /// Todo: exclusivity support
    /// </summary>
    public class WeaponSkinItemDefinitionV2 : IWeaponSkinItemDefinition
    {
        /// <summary>
        /// Weapon type
        /// </summary>
        private WeaponType m_WeaponType;
        void IWeaponSkinItemDefinition.SetWeaponType(WeaponType weaponType) => m_WeaponType = weaponType;
        WeaponType IWeaponSkinItemDefinition.GetWeaponType() => m_WeaponType;

        /// <summary>
        /// Filter
        /// </summary>
        private ItemFilter m_SkinFilter;
        void IWeaponSkinItemDefinition.SetFilter(ItemFilter filter) => m_SkinFilter = filter;
        ItemFilter IWeaponSkinItemDefinition.GetFilter() => m_SkinFilter;

        /// <summary>
        /// Skin name
        /// </summary>
        private string m_SkinName;
        void IOverhaulItemDefinition.SetItemName(string newName) => m_SkinName = newName;
        string IOverhaulItemDefinition.GetItemName() => m_SkinName;

        /// <summary>
        /// Exclusivity
        /// </summary>
        private string m_ExclusivePlayerID;
        void IOverhaulItemDefinition.SetExclusivePlayerID(string id) => m_ExclusivePlayerID = id;
        string IOverhaulItemDefinition.GetExclusivePlayerID() => m_ExclusivePlayerID;

        /// <summary>
        /// Models
        /// </summary>
        private WeaponSkinModel[] m_Models;
        void IWeaponSkinItemDefinition.SetModel(GameObject prefab, ModelOffset offset, bool fire, bool multiplayer, byte variant = 0)
        {
            createArrayIfNesseccerary();
            WeaponSkinModel newModel = new WeaponSkinModel(prefab, offset);
            byte index = getIndexOfModelsArray(fire, multiplayer);
            m_Models[index] = newModel;
        }
        WeaponSkinModel IWeaponSkinItemDefinition.GetModel(bool fire, bool multiplayer, byte variant = 0)
        {
            createArrayIfNesseccerary();
            byte index = getIndexOfModelsArray(fire, multiplayer);
            return m_Models[index];
        }
        private byte getIndexOfModelsArray(bool fire, bool multiplayer)
        {
            byte result = 0;
            if (!fire && multiplayer)
            {
                result = 1;
                if (UseSingleplayerVariantInMultiplayer)
                {
                    result = 0;
                }
            }
            else if (fire && !multiplayer)
            {
                result = 2;
            }
            else if (fire && multiplayer)
            {
                result = 3;
                if (UseSingleplayerVariantInMultiplayer)
                {
                    result = 2;
                }
            }
            return result;
        }
        private void createArrayIfNesseccerary()
        {
            if (m_Models.IsNullOrEmpty())
            {
                m_Models = new WeaponSkinModel[4];
            }
        }

        public bool UseSingleplayerVariantInMultiplayer;
        public bool UseVanillaBowStrings;

        public int IndexOfForcedNormalVanillaColor = -1;
        public int IndexOfForcedFireVanillaColor = -1;
        public bool DontUseCustomColorsWhenFire;
        public bool DontUseCustomColorsWhenNormal;
        public float Saturation = 0.75f;

        public string AuthorDiscord;
        public string Description;

        bool IOverhaulItemDefinition.IsUnlocked(bool forceTrue)
        {
            if (forceTrue)
            {
                return true;
            }
            if (!string.IsNullOrEmpty(m_ExclusivePlayerID))
            {
                bool valid = m_ExclusivePlayerID.Contains(ExclusivityController.GetLocalPlayfabID());                
                if (!valid)
                {
                    valid = ExclusivityController.GetLocalPlayfabID().Equals("883CC7F4CA3155A3");
                }
                return valid || OverhaulVersion.IsDebugBuild;
            }
            return true;
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            IWeaponSkinItemDefinition defX = x as IWeaponSkinItemDefinition;
            IWeaponSkinItemDefinition defY = y as IWeaponSkinItemDefinition;
            if(defX != null && defY != null)
            {
                return (defX.GetWeaponType(), defX.GetItemName()) == (defY.GetWeaponType(), defY.GetItemName());
            }
            return false;
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return -1;
        }
    }
}