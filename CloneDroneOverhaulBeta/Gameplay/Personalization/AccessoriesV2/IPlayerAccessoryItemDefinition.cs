using OverhaulAPI;
using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public interface IPlayerAccessoryItemDefinition : IOverhaulItemDefinition
    {
        void SetModel(GameObject prefab);
        GameObject GetModel(bool fire, bool multiplayer);

        void SetFilter(ItemFilter filter);
        ItemFilter GetFilter();

        void SetBodypartType(MechBodyPartType weaponType);
        MechBodyPartType GetBodypartType();
    }
}
