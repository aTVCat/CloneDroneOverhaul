using OverhaulAPI;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class RobotAccessoryItemDefinition : IOverhaulItemDefinition
    {
        public string AccessoryName;

        public SerializeTransform TransformInfo;

        public GameObject AccessoryPrefab;

        public MechBodyPartType PartType;

        public List<RobotAccessoryTransformData> TransformData;

        public string PrefabName;

        public void SetPrefabUp()
        {
            PrefabName = "P_Acc_" + PartType.ToString() + "_" + AccessoryName;
            AccessoryPrefab = AssetController.GetAsset<GameObject>(PrefabName, Enumerators.EModAssetBundlePart.Accessories);
        }

        public void SetTransformsUp()
        {
            TransformData = new List<RobotAccessoryTransformData>(MultiplayerCharacterCustomizationManager.Instance.CharacterModels.Count);
            for (int i = 0; i < MultiplayerCharacterCustomizationManager.Instance.CharacterModels.Count; i++)
            {
                TransformData.Add(null);
            }
        }

        public int GetIndexOfCharacterModel(in FirstPersonMover mover)
        {
            string modelName = mover.GetCharacterModel().gameObject.name.Replace("(Clone)", string.Empty);
            int indexOfModel = -1;
            int i = 0;
            do
            {
                if (MultiplayerCharacterCustomizationManager.Instance.CharacterModels[i].CharacterModelPrefab.name == modelName)
                {
                    indexOfModel = i;
                    break;
                }
                i++;
            } while (i < MultiplayerCharacterCustomizationManager.Instance.CharacterModels.Count);
            return indexOfModel;
        }

        public SerializeTransform GetTranformForFPM(in FirstPersonMover mover)
        {
            if (!mover.HasCharacterModel())
            {
                return null;
            }

            int indexOfModel = GetIndexOfCharacterModel(mover);
            if (indexOfModel == -1)
            {
                return TransformInfo;
            }

            if (TransformData[indexOfModel] == null)
            {
                TransformData[indexOfModel] = RobotAccessoryTransformData.GetData<RobotAccessoryTransformData>(indexOfModel + "_" + AccessoryName + ".json", true, "Accessories/");
            }
            return TransformData[indexOfModel].Data;
        }

        public void SaveTransformForAccessory(in RobotAccessoryBehaviour behaviour)
        {
            if (behaviour.Owner == null)
            {
                return;
            }

            int index = GetIndexOfCharacterModel(behaviour.Owner);
            if (index == -1)
            {
                OverhaulDebugController.Print("Incorrect character model!!", Color.red);
                return;
            }

            RobotAccessoryTransformData data = TransformData[index];
            if (data == null)
            {
                TransformData[index] = RobotAccessoryTransformData.GetData<RobotAccessoryTransformData>(index + "_" + AccessoryName + ".json", true, "Accessories/");
            }
            data.AccessoryName = AccessoryName;
            data.CharacterModelIndex = index;
            data.Data = SerializeTransform.SerializeTheTransform(behaviour.transform);
            data.SaveTransforms();
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
            return true;
        }
    }
}