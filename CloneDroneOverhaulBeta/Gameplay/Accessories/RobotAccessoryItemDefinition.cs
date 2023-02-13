using OverhaulAPI;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class RobotAccessoryItemDefinition : IOverhaulItemDefinition
    {
        public string AccessoryName;

        public SerializeTransform Transforminfo;

        public GameObject AccessoryPrefab;

        public MechBodyPartType PartType;

        public void SetPrefabUp()
        {
            string prefabName = "P_Acc_" + PartType.ToString() + "_" + AccessoryName;
            AccessoryPrefab = AssetController.GetAsset<GameObject>(prefabName, Enumerators.EModAssetBundlePart.Accessories);
        }

        string IOverhaulItemDefinition.ItemName()
        {
            return AccessoryName;
        }

        bool IOverhaulItemDefinition.IsUnlocked(bool forceTrue)
        {
            if (OverhaulVersion.IsDebugBuild || forceTrue)
            {
                return true;
            }
            return false;
        }
    }
}