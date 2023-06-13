using Newtonsoft.Json;
using OverhaulAPI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CDOverhaul.Gameplay.Outfits
{
    public abstract class AccessoryItem
    {
        public const string NoDescProvidedString = "No description provided.";
        public const string NoAuthorString = "TBA";

        public string Name { get; private set; }

        private string m_Description;
        public string Description => string.IsNullOrEmpty(m_Description) ? NoDescProvidedString : m_Description;

        private string m_Author;
        public string Author
        {
            get => string.IsNullOrEmpty(m_Author) ? NoAuthorString : m_Author;
            set => m_Author = value;
        }

        public AccessoryType Type { get; private set; }

        public MechBodyPartType BodyPart { get; private set; }

        /// <summary>
        /// Character model names - Offset
        /// </summary>
        public Dictionary<string, ModelOffset> Offsets;

        /// <summary>
        /// A model of accessory
        /// </summary>
        public GameObject Prefab;

        public string AllowedPlayers;
        public bool IsUnlocked()
        {
            if (OverhaulVersion.IsDebugBuild || string.IsNullOrEmpty(AllowedPlayers))
                return true;

            string localID = ExclusivityController.GetLocalPlayFabID();
            bool isUnlocked = AllowedPlayers.Contains(localID);
            if (!isUnlocked && OverhaulFeatureAvailabilitySystem.BuildImplements.AllowDeveloperUseAllSkins)
                isUnlocked = localID.Equals("883CC7F4CA3155A3");

            return isUnlocked;
        }

        public abstract void SetUpPrefab(GameObject prefab);

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
        /// <param name="type"></param>
        /// <returns></returns>
        public static T NewAccessory<T>(string name, string description, AccessoryType type, MechBodyPartType partType) where T : AccessoryItem
        {
            T item = Activator.CreateInstance<T>();
            item.Name = name;
            item.m_Description = description;
            item.Type = type;
            item.BodyPart = partType;
            return item;
        }

    }
}
