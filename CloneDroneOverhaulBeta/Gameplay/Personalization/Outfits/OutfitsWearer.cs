using CDOverhaul.Visuals;
using OverhaulAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.Outfits
{
    public class OutfitsWearer : PersonalizationItemsWearer
    {
        private Dictionary<string, GameObject> m_SpawnedOutfitItems = new Dictionary<string, GameObject>();

        public override void Start()
        {
            base.Start();
            RefreshItems();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            DestroyItems();

            m_SpawnedOutfitItems = null;
        }

        protected override void OnDeath()
        {
            DestroyItems();
        }

        public override void RefreshItems()
        {
            DestroyItems();

            if (!Owner || !OverhaulGamemodeManager.SupportsOutfits())
                return;

            OutfitsController outfitsController = OverhaulController.Get<OutfitsController>();
            if (!outfitsController)
                return;

            string equippedItems = string.Empty;
            if (PlayerInformation)
            {
                Hashtable hashtable = PlayerInformation.Hashtable;
                if (hashtable != null && hashtable.ContainsKey("Outfits.Equipped"))
                    equippedItems = hashtable["Outfits.Equipped"].ToString();
            }
            else if (!GameModeManager.IsMultiplayer() && (OutfitsController.AllowEnemiesWearAccesories || IsOwnerPlayer()))
                equippedItems = OutfitsController.EquippedAccessories;

            foreach (OutfitItem accessoryItem in outfitsController.GetItemsWithSaveString(equippedItems))
            {
                if (accessoryItem.ItemModel == null || !accessoryItem.ItemModel.LoadAsset() || m_SpawnedOutfitItems.ContainsKey(accessoryItem.Name))
                    continue;

                Transform bodyPartTransform = Owner.GetBodyPartParent(accessoryItem.BodyPart);
                if (!bodyPartTransform)
                    continue;

                GameObject instantiatedPrefab = Instantiate(accessoryItem.ItemModel.Asset as GameObject, bodyPartTransform);
                SetOffset(instantiatedPrefab, accessoryItem);
                m_SpawnedOutfitItems.Add(accessoryItem.Name, instantiatedPrefab);
            }
        }

        public void DestroyItems()
        {
            if (m_SpawnedOutfitItems.IsNullOrEmpty())
                return;

            foreach (string key in m_SpawnedOutfitItems.Keys)
            {
                GameObject toDestroy = m_SpawnedOutfitItems[key];
                if (toDestroy)
                    Destroy(toDestroy);
            }
            m_SpawnedOutfitItems.Clear();
        }

        public void SetOffset(GameObject outfitItem, OutfitItem item)
        {
            if (!outfitItem || item == null || !Owner || !Owner.HasCharacterModel())
                return;

            string characterModelName = Owner.GetCharacterModel().gameObject.name;
            if (string.IsNullOrEmpty(characterModelName) || item.Offsets.IsNullOrEmpty() || !item.Offsets.ContainsKey(characterModelName))
            {
                outfitItem.SetActive(false);
                return;
            }

            ModelOffset modelOffset = item.Offsets[characterModelName];
            if (modelOffset.OffsetLocalScale == Vector3.zero)
            {
                outfitItem.SetActive(false);
                return;
            }

            Transform outfitItemTransform = outfitItem.transform;
            outfitItemTransform.localPosition = modelOffset.OffsetPosition;
            outfitItemTransform.localEulerAngles = modelOffset.OffsetEulerAngles;
            outfitItemTransform.localScale = modelOffset.OffsetLocalScale;
        }

        private void Update()
        {
            if (Time.frameCount % 10 == 0 && !m_SpawnedOutfitItems.IsNullOrEmpty() && Owner.IsMainPlayer())
            {
                foreach (GameObject gm in m_SpawnedOutfitItems.Values)
                {
                    if (!gm)
                        continue;

                    gm.SetActive(!ViewModesController.IsFirstPersonModeEnabled || PhotoManager.Instance.IsInPhotoMode());
                }
            }
        }
    }
}