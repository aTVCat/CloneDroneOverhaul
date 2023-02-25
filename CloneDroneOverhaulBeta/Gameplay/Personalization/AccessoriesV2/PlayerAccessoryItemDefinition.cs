using OverhaulAPI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class PlayerAccessoryItemDefinition : IPlayerAccessoryItemDefinition
    {
        /// <summary>
        /// Filter
        /// </summary>
        private ItemFilter m_Filter;
        void IPlayerAccessoryItemDefinition.SetFilter(ItemFilter filter) => m_Filter = filter;
        ItemFilter IPlayerAccessoryItemDefinition.GetFilter() => m_Filter;

        /// <summary>
        /// Item name
        /// </summary>
        private string m_ItemName;
        void IOverhaulItemDefinition.SetItemName(string newName) => m_ItemName = newName;
        string IOverhaulItemDefinition.GetItemName() => m_ItemName;

        /// <summary>
        /// Exclusivity
        /// </summary>
        private string m_ExclusivePlayerID;
        void IOverhaulItemDefinition.SetExclusivePlayerID(string id) => m_ExclusivePlayerID = id;
        string IOverhaulItemDefinition.GetExclusivePlayerID() => m_ExclusivePlayerID;

        /// <summary>
        /// Body part
        /// </summary>
        private MechBodyPartType m_BodyPartType;
        void IPlayerAccessoryItemDefinition.SetBodypartType(MechBodyPartType bodyPart) => m_BodyPartType = bodyPart;
        MechBodyPartType IPlayerAccessoryItemDefinition.GetBodypartType() => m_BodyPartType;

        /// <summary>
        /// The model
        /// </summary>
        private IPlayerAccessoryModel m_Model;
        void IPlayerAccessoryItemDefinition.SetModel(IPlayerAccessoryModel model) => m_Model = model;
        IPlayerAccessoryModel IPlayerAccessoryItemDefinition.GetModel() => m_Model;

        /// <summary>
        /// Offsets
        /// </summary>
        private Dictionary<string, ModelOffset> m_Offsets;

        public void CreateDefaultOffsets()
        {
            m_Offsets = new Dictionary<string, ModelOffset>(MultiplayerCharacterCustomizationManager.Instance.CharacterModels.Count);
            for (int i = 0; i < MultiplayerCharacterCustomizationManager.Instance.CharacterModels.Count; i++)
            {
                m_Offsets.Add(MultiplayerCharacterCustomizationManager.Instance.CharacterModels[i].CharacterModelPrefab.gameObject.name.Replace("(Clone)", string.Empty),
                    ModelOffset.Default);
            }
        }

        public void LoadOffsets()
        {
            createMissingOffsets();
        }

        public void SaveOffsets()
        {
            PlayerAccessoryOffsetData data = PlayerAccessoryOffsetData.GetData<PlayerAccessoryOffsetData>(m_ItemName + "Offsets", true, PlayerAccessoryOffsetData.FolderName);
            data.Offsets = m_Offsets;
            data.AccessoryItemName = m_ItemName;
            data.SaveData(true, PlayerAccessoryOffsetData.FolderName);
        }

        private void createMissingOffsets()
        {
            PlayerAccessoryOffsetData data = PlayerAccessoryOffsetData.GetData<PlayerAccessoryOffsetData>(m_ItemName + "Offsets", true, PlayerAccessoryOffsetData.FolderName);
            if (data.Offsets.IsNullOrEmpty())
            {
                CreateDefaultOffsets();
                data.Offsets = m_Offsets;
                data.AccessoryItemName = m_ItemName;
                data.SaveData(true, PlayerAccessoryOffsetData.FolderName);
            }
            else
            {
                m_Offsets = data.Offsets;
            }
        }

        public bool IsFirstPersonMoverSupported(FirstPersonMover firstPersonMover)
        {
            if(firstPersonMover == null || !firstPersonMover.HasCharacterModel())
            {
                return false;
            }
            string characterModelName = firstPersonMover.GetCharacterModel().gameObject.name.Replace("(Clone)", string.Empty);
            if (!m_Offsets.ContainsKey(characterModelName))
            {
                return false;
            }
            return true;
        }

        public ModelOffset GetFirstPersonMoverOffset(FirstPersonMover firstPersonMover)
        {
            if (!IsFirstPersonMoverSupported(firstPersonMover))
            {
                return null;
            }
            string characterModelName = firstPersonMover.GetCharacterModel().gameObject.name.Replace("(Clone)", string.Empty);
            return m_Offsets[characterModelName];
        }

        bool IOverhaulItemDefinition.IsUnlocked(bool forceTrue)
        {
            return true;
        }
    }
}
