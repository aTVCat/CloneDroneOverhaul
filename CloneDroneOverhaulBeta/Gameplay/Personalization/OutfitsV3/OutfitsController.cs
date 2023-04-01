using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CDOverhaul.Gameplay.Outfits
{
    public class OutfitsController : OverhaulGameplayController
    {
        private static readonly List<AccessoryItem> m_Accessories = new List<AccessoryItem>();

        public override void Initialize()
        {
            base.Initialize();

            if (!OverhaulSessionController.GetKey<bool>("HasAddedAccessories"))
            {
                OverhaulSessionController.SetKey("HasAddedAccessories", true);
                addAccessories();
            }
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
            {
                return;
            }

            firstPersonMover.gameObject.AddComponent<OutfitsWearer>();
        }

        private void addAccessories()
        {
            AddAccessory<DefaultAccessoryItem>("Igrok's hat", "", AccessoryType.Attached, MechBodyPartType.Head);
        }

        public void AddAccessory<T>(string accessoryName,
            string assetName,
            AccessoryType accessoryType,
            MechBodyPartType accessoryBodyPart,
            string descriptionFile = null) where T : AccessoryItem
        {
            string desc = null;
            if (!string.IsNullOrEmpty(descriptionFile))
            {
                string path = OverhaulMod.Core.ModDirectory + "Assets/OutfitDescriptions/" + descriptionFile + ".txt";
                bool fileExists = File.Exists(path);
                if (!fileExists)
                {
                    return;
                }

                StreamReader r = File.OpenText(path);
                desc = r.ReadToEnd();
                r.Close();
            }


            AccessoryItem item = AccessoryItem.NewAccessory<T>(accessoryName, desc, accessoryType, accessoryBodyPart);
            if(!string.IsNullOrEmpty(assetName)) item.Prefab = AssetsController.GetAsset(assetName, OverhaulAssetsPart.Accessories);
            m_Accessories.Add(item);
        }
    }
}