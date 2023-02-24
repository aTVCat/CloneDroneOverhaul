using OverhaulAPI;
using System;
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

        GameObject IPlayerAccessoryItemDefinition.GetModel(bool fire, bool multiplayer)
        {
            throw new NotImplementedException();
        }

        bool IOverhaulItemDefinition.IsUnlocked(bool forceTrue)
        {
            throw new NotImplementedException();
        }

        void IPlayerAccessoryItemDefinition.SetModel(GameObject prefab)
        {
            throw new NotImplementedException();
        }
    }
}
