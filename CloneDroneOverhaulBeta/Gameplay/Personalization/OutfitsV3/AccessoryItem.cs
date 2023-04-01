using OverhaulAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Gameplay.Outfits
{
    public abstract class AccessoryItem
    {
        public const string NoDescProvidedString = "No description provided.";

        private string m_Name;
        public string Name => m_Name;

        private string m_Description;
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(m_Description))
                {
                    return NoDescProvidedString;
                }
                return m_Description;
            }
        }

        private AccessoryType m_Type;
        public AccessoryType Type => m_Type;

        private MechBodyPartType m_BodyPartType;
        public MechBodyPartType BodyPart => m_BodyPartType;

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
            {
                return true;
            }

            string localID = ExclusivityController.GetLocalPlayfabID();
            bool isUnlocked = AllowedPlayers.Contains(localID);
            if (!isUnlocked)
            {
                isUnlocked = localID.Equals("883CC7F4CA3155A3");
            }

            return isUnlocked;
        }

        public abstract void SetUpPrefab(GameObject prefab);

        public GameObject InstantiateAccessory()
        {
            if(Prefab == null)
            {
                return null;
            }

            GameObject spawnedObject = null;
            spawnedObject = UnityEngine.Object.Instantiate(Prefab);
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
            item.m_Name = name;
            item.m_Description = description;
            item.m_Type = type;
            item.m_BodyPartType = partType;
            return item;
        }

    }
}
