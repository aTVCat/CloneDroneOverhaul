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
        void IWeaponSkinItemDefinition.SetWeaponType(WeaponType weaponType)
        {
            m_WeaponType = weaponType;
        }

        WeaponType IWeaponSkinItemDefinition.GetWeaponType()
        {
            return m_WeaponType;
        }

        /// <summary>
        /// Filter
        /// </summary>
        private ItemFilter m_SkinFilter;
        void IWeaponSkinItemDefinition.SetFilter(ItemFilter filter)
        {
            m_SkinFilter = filter;
        }

        ItemFilter IWeaponSkinItemDefinition.GetFilter()
        {
            return m_SkinFilter;
        }

        /// <summary>
        /// Skin name
        /// </summary>
        private string m_SkinName;
        void IOverhaulItemDefinition.SetItemName(string newName)
        {
            m_SkinName = newName;
        }

        string IOverhaulItemDefinition.GetItemName()
        {
            return m_SkinName;
        }

        /// <summary>
        /// Exclusivity
        /// </summary>
        private string m_ExclusivePlayerID;
        void IOverhaulItemDefinition.SetExclusivePlayerID(string id)
        {
            m_ExclusivePlayerID = id;
        }

        string IOverhaulItemDefinition.GetExclusivePlayerID()
        {
            return m_ExclusivePlayerID;
        }

        public string OverrideName;
        public bool HasNameOverride => !string.IsNullOrEmpty(OverrideName);

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

        public bool IsDeveloperItem;
        public bool IsDevItemUnlocked => Equals(ExclusivityController.GetLocalPlayfabID(), "883CC7F4CA3155A3");

        public bool UseSingleplayerVariantInMultiplayer;
        public bool UseVanillaBowStrings;

        public int IndexOfForcedNormalVanillaColor = -1;
        public int IndexOfForcedFireVanillaColor = -1;
        public bool DontUseCustomColorsWhenFire;
        public bool DontUseCustomColorsWhenNormal;
        public float Saturation = 0.75f;
        public float Multiplier = 1f;

        public string AuthorDiscord;
        public string Description;

        public bool IsImportedSkin;

        bool IOverhaulItemDefinition.IsUnlocked(bool forceTrue)
        {
            if (forceTrue)
            {
                return true;
            }

            if (OverhaulVersion.IsDebugBuild || string.IsNullOrEmpty(m_ExclusivePlayerID))
            {
                return true;
            }

            string localID = ExclusivityController.GetLocalPlayfabID();
            bool isUnlocked = m_ExclusivePlayerID.Contains(localID);
            if (!isUnlocked)
            {
                isUnlocked = localID.Equals("883CC7F4CA3155A3");
            }
            return isUnlocked;
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            return x is IWeaponSkinItemDefinition defX && y is IWeaponSkinItemDefinition defY
&& (defX.GetWeaponType(), defX.GetItemName()) == (defY.GetWeaponType(), defY.GetItemName());
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return -1;
        }
    }
}