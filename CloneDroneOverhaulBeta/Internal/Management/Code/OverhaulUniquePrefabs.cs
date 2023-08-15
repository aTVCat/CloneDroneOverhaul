using PicaVoxel;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulUniquePrefabs
    {
        private static Transform s_ObjectsContainer;
        public static Transform ObjectsContainer
        {
            get
            {
                if (!s_ObjectsContainer)
                {
                    Transform container = new GameObject("OverhaulUniquePrefabsContainer").transform;
                    container.gameObject.SetActive(false);
                    s_ObjectsContainer = container;
                }
                return s_ObjectsContainer;
            }
        }


        private static Volume s_EmptyVolume;
        public static Volume EmptyVolume
        {
            get
            {
                if (!s_EmptyVolume)
                {
                    MultiplayerCharacterCustomizationManager characterCustomization = MultiplayerCharacterCustomizationManager.Instance;
                    if (!characterCustomization || characterCustomization.CharacterModels.IsNullOrEmpty())
                        return null;

                    CharacterModelCustomizationEntry customizationEntry = characterCustomization.CharacterModels[0];
                    if (customizationEntry == null || !customizationEntry.CharacterModelPrefab)
                        return null;

                    WeaponModel swordWeaponModel = customizationEntry.CharacterModelPrefab.GetWeaponModel(WeaponType.Sword);
                    if (!swordWeaponModel)
                        return null;

                    Transform theModel = swordWeaponModel.transform.FindChildRecursive("model_bluetest (1)");
                    if (!theModel)
                        return null;

                    Transform modelClone = UnityEngine.Object.Instantiate(theModel, ObjectsContainer);
                    modelClone.gameObject.name = "SwordModel";

                    ReplaceVoxelColor[] replaceVoxelColorComponents = modelClone.GetComponents<ReplaceVoxelColor>();
                    if (!replaceVoxelColorComponents.IsNullOrEmpty())
                    {
                        foreach (ReplaceVoxelColor replaceVoxelColor in replaceVoxelColorComponents)
                            UnityEngine.Object.Destroy(replaceVoxelColor);
                    }

                    s_EmptyVolume = modelClone.GetComponent<Volume>();
                }
                return s_EmptyVolume;
            }
        }
    }
}
