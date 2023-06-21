using OverhaulAPI;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
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
            if (!fire && multiplayer && !UseSingleplayerVariantInMultiplayer)
                result = 1;
            else if (fire && !multiplayer)
                result = 2;
            else if (fire && multiplayer)
                result = UseSingleplayerVariantInMultiplayer ? (byte)2 : (byte)3;
            return result;
        }
        private void createArrayIfNesseccerary()
        {
            if (m_Models.IsNullOrEmpty())
                m_Models = new WeaponSkinModel[4];
        }

        public bool IsDeveloperItem;
        public bool IsDevItemUnlocked => Equals(ExclusivityController.GetLocalPlayFabID(), "883CC7F4CA3155A3");

        public bool UseSingleplayerVariantInMultiplayer;
        public bool UseVanillaBowStrings;

        public int IndexOfForcedNormalVanillaColor = -1;
        public int IndexOfForcedFireVanillaColor = -1;
        public bool DontUseCustomColorsWhenFire;
        public bool DontUseCustomColorsWhenNormal;
        public float Saturation = 0.75f;
        public float Multiplier = 1f;

        public string ReparentToBodypart;

        public string AuthorDiscord;
        public string Description;

        public bool IsImportedSkin;
        public string OverrideAssetBundle;

        public string CollideWithEnvironmentVFXAssetName;
        public int CountOfPreparedPooledPrefabObjects;

        bool IOverhaulItemDefinition.IsUnlocked(bool forceTrue)
        {
            if (forceTrue || OverhaulVersion.IsDebugBuild || string.IsNullOrEmpty(m_ExclusivePlayerID)) 
                return true;

            string localPlayFabID = ExclusivityController.GetLocalPlayFabID();
            bool result = m_ExclusivePlayerID.Contains(localPlayFabID);

            // Force unlock skin if the user is CDO developer
            if (!result && OverhaulFeatureAvailabilitySystem.ImplementedInBuild.AllowDeveloperUseAllSkins)
                result = localPlayFabID.Equals("883CC7F4CA3155A3");

            // Check discord user id (Not the best idea)
            if (!result && OverhaulDiscordController.Instance && OverhaulDiscordController.SuccessfulInitialization)
            {
                long id = OverhaulDiscordController.Instance.UserID;
                if(id != -1)
                    result = m_ExclusivePlayerID.Contains(id.ToString());
            }

            return result;
        }

        public bool IsUnlockedForPlayer(FirstPersonMover player)
        {
            if (!player)
                return false;

            if (string.IsNullOrEmpty(m_ExclusivePlayerID) || player.HasPermissionToUseLockedStuff())
                return true;

            string playfabID = player.GetPlayFabID();
            if (string.IsNullOrEmpty(playfabID))
                return false;

            return m_ExclusivePlayerID.Contains(playfabID);
        }

        bool IEqualityComparer.Equals(object x, object y) => x is IWeaponSkinItemDefinition defX && y is IWeaponSkinItemDefinition defY && (defX.GetWeaponType(), defX.GetItemName()) == (defY.GetWeaponType(), defY.GetItemName());
        int IEqualityComparer.GetHashCode(object obj) => -1;
    }
}