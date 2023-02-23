using OverhaulAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class PlayerAccessoryItemDefinition : IPlayerAccessoryItemDefinition
    {
        MechBodyPartType IPlayerAccessoryItemDefinition.GetBodypartType()
        {
            throw new NotImplementedException();
        }

        string IOverhaulItemDefinition.GetExclusivePlayerID()
        {
            throw new NotImplementedException();
        }

        ItemFilter IPlayerAccessoryItemDefinition.GetFilter()
        {
            throw new NotImplementedException();
        }

        string IOverhaulItemDefinition.GetItemName()
        {
            throw new NotImplementedException();
        }

        GameObject IPlayerAccessoryItemDefinition.GetModel(bool fire, bool multiplayer)
        {
            throw new NotImplementedException();
        }

        bool IOverhaulItemDefinition.IsUnlocked(bool forceTrue)
        {
            throw new NotImplementedException();
        }

        void IPlayerAccessoryItemDefinition.SetBodypartType(MechBodyPartType weaponType)
        {
            throw new NotImplementedException();
        }

        void IOverhaulItemDefinition.SetExclusivePlayerID(string id)
        {
            throw new NotImplementedException();
        }

        void IPlayerAccessoryItemDefinition.SetFilter(ItemFilter filter)
        {
            throw new NotImplementedException();
        }

        void IOverhaulItemDefinition.SetItemName(string newName)
        {
            throw new NotImplementedException();
        }

        void IPlayerAccessoryItemDefinition.SetModel(GameObject prefab)
        {
            throw new NotImplementedException();
        }
    }
}
