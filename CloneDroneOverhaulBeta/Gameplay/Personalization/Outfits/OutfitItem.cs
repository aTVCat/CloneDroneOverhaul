using Newtonsoft.Json;
using OverhaulAPI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CDOverhaul.Gameplay.Outfits
{
    public class OutfitItem : PersonalizationItem
    {
        public string BodyPart;

        /// <summary>
        /// Character model names - Offset
        /// </summary>
        public Dictionary<string, ModelOffset> Offsets;

        /// <summary>
        /// A model of accessory
        /// </summary>
        [NonSerialized]
        public GameObject Prefab;

        public string ExclusiveFor;
        public bool IsUnlocked()
        {
            if (OverhaulVersion.IsDebugBuild || string.IsNullOrEmpty(ExclusiveFor))
                return true;

            string localID = OverhaulPlayerIdentifier.GetLocalPlayFabID();
            bool isUnlocked = ExclusiveFor.Contains(localID);
            if (!isUnlocked && OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IS_DEVELOPER_ALLOWED_TO_USE_LOCKED_STUFF)
                isUnlocked = localID.Equals("883CC7F4CA3155A3");

            return isUnlocked;
        }

        public void SetUpOffsets()
        {
            string path = "Assets/AccessoriesOffsets/" + Name + "_Offsets.json";
            bool hasOffsetsFile = File.Exists(OverhaulMod.Core.ModDirectory + path);
            if (!hasOffsetsFile)
            {
                MultiplayerCharacterCustomizationManager multiplayerCharacterCustomization = MultiplayerCharacterCustomizationManager.Instance;
                if (multiplayerCharacterCustomization == null || multiplayerCharacterCustomization.CharacterModels.IsNullOrEmpty())
                    return;

                Offsets = new Dictionary<string, ModelOffset>();
                foreach (CharacterModelCustomizationEntry entry in multiplayerCharacterCustomization.CharacterModels)
                {
                    if (entry == null || entry.CharacterModelPrefab == null)
                        continue;

                    string characterModelName = entry.CharacterModelPrefab.gameObject.name + "(Clone)";
                    Offsets.Add(characterModelName, new ModelOffset(Vector3.zero, Vector3.zero, Vector3.one));
                }
                SaveOffsets();
            }
            else
            {
                string text = OverhaulCore.ReadText(OverhaulMod.Core.ModDirectory + path);
                Offsets = JsonConvert.DeserializeObject<Dictionary<string, ModelOffset>>(text);
            }
        }

        public void SaveOffsets()
        {
            OverhaulCore.WriteText(OverhaulMod.Core.ModDirectory + "Assets/AccessoriesOffsets/" + Name + "_Offsets.json", JsonConvert.SerializeObject(Offsets));
        }

        public GameObject InstantiateAccessory()
        {
            if (Prefab == null)
                return null;

            GameObject spawnedObject = UnityEngine.Object.Instantiate(Prefab);
            spawnedObject.name = spawnedObject.name.Replace("(Clone)", string.Empty);
            spawnedObject.SetActive(true);
            return spawnedObject;
        }

        /// <summary>
        /// Create instance of accessory item including its name, description and type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="partType"></param>
        /// <returns></returns>
        public static OutfitItem NewAccessory(string name, string partType)
        {
            OutfitItem item = new OutfitItem
            {
                Name = name,
                BodyPart = partType
            };
            return item;
        }
    }
}
