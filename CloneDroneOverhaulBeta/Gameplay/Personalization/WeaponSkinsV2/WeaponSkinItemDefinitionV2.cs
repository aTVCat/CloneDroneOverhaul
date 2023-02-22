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
        private WeaponSkinItemFilter m_SkinFilter;
        void IWeaponSkinItemDefinition.SetFilter(WeaponSkinItemFilter filter) => m_SkinFilter = filter;
        WeaponSkinItemFilter IWeaponSkinItemDefinition.GetFilter() => m_SkinFilter;

        /// <summary>
        /// Skin name
        /// </summary>
        private string m_SkinName;
        void IOverhaulItemDefinition.SetItemName(string newName) => m_SkinName = newName;
        string IOverhaulItemDefinition.GetItemName() => m_SkinName;

        /// <summary>
        /// Models
        /// </summary>
        private WeaponSkinModel[] m_Models;
        void IWeaponSkinItemDefinition.SetModel(GameObject prefab, WeaponSkinModelOffset offset, bool fire, bool multiplayer)
        {
            createArrayIfNesseccerary();
            WeaponSkinModel newModel = new WeaponSkinModel(prefab, offset);
            byte index = getIndexOfModelsArray(fire, multiplayer);
            m_Models[index] = newModel;
        }
        WeaponSkinModel IWeaponSkinItemDefinition.GetModel(bool fire, bool multiplayer)
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
            }
            else if (fire && !multiplayer)
            {
                result = 2;
            }
            else if (fire && multiplayer)
            {
                result = 3;
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

        bool IOverhaulItemDefinition.IsUnlocked(bool forceTrue)
        {
            throw new System.NotImplementedException();
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